// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

using System;
using System.Collections.Generic;
using System.Linq;
using ProcurandoApartamento.Dto;

namespace ProcurandoApartamento.Domain.Services;

public static class QuadraService
{
    public static void ValidaSeExisteEstabelecimentoNaQuadra(List<string> estabelecimentosDesejados, 
        IGrouping<int, Apartamento> quadras, ICollection<MelhorQuadraDto> listaDasMelhoresQuadras)
    {
        var melhorQuadra = new MelhorQuadraDto { Quadra = quadras.Key };

        int ordemDePrioridade = 1;
        foreach (string estabelecimento in estabelecimentosDesejados)
        {
            bool apartamentoComEstabelecimentoDesejado = quadras.Any(q =>
                string.Equals(q.Estabelecimento, estabelecimento, StringComparison.CurrentCultureIgnoreCase)
                && q.ApartamentoDisponivel
                && q.EstabelecimentoExiste);

            if (apartamentoComEstabelecimentoDesejado)
                melhorQuadra.AdicionaApartamentoDisponivel(ordemDePrioridade);

            ordemDePrioridade++;
        }

        if (melhorQuadra.ApartamentosDisponiveis.Count > 0) listaDasMelhoresQuadras.Add(melhorQuadra);
    }

    public static List<MelhorQuadraDto> ExisteQuadrasComMesmaQuantidadeDeEstabelecimentos(
        this IReadOnlyCollection<MelhorQuadraDto> listaDasMelhoresQuadras)
    {
        var ordenadoPorQuantidadeDeApartamentosDisponiveisPorQuadra = 
            listaDasMelhoresQuadras.OrderByDescending(e => e.ApartamentosDisponiveis.Count);

        int quantidadeMaximaDeApartamentosEncontradosPorQuadra =
            ordenadoPorQuantidadeDeApartamentosDisponiveisPorQuadra.First().ApartamentosDisponiveis.Count;

        var quadrasComMesmaQuantidadeDeEstabelecimentos = listaDasMelhoresQuadras
            .Where(mq => 
                mq.ApartamentosDisponiveis.Count == quantidadeMaximaDeApartamentosEncontradosPorQuadra)
            .ToList();
        return quadrasComMesmaQuantidadeDeEstabelecimentos;
    }

    public static List<MelhorQuadraDto> ExisteQuadrasComMesmaPrioridade(
        this IEnumerable<MelhorQuadraDto> quadrasComMesmaQuantidadeDeEstabelecimentos)
    {
        var ordenadoPorPrioridadeDeEstabelecimento = quadrasComMesmaQuantidadeDeEstabelecimentos
            .OrderBy(mq =>
                mq.ApartamentosDisponiveis.Min(ad => ad.Prioridade))
            .ToList();

        int maiorPrioridadePorEstabelecimento = ordenadoPorPrioridadeDeEstabelecimento
            .First().ApartamentosDisponiveis.Count;

        var quadrasComMesmaPrioridade = ordenadoPorPrioridadeDeEstabelecimento
            .Where(mq => 
                mq.ApartamentosDisponiveis.Exists(x => 
                    x.Prioridade == maiorPrioridadePorEstabelecimento))
            .ToList();
        return quadrasComMesmaPrioridade;
    }

    public static string RecuperaQuadraMaisProximaDoFimDaRua(
        this IEnumerable<MelhorQuadraDto> ordenadoPorPrioridadeDeEstabelecimento)
    {
        int quadraMaisProximaDoFinalDaRua = ordenadoPorPrioridadeDeEstabelecimento.Max(c => c.Quadra);
        return MelhorQuadraToString(quadraMaisProximaDoFinalDaRua);
    }
    public static string RecuperaQuadraMaisProximaDoFimDaRua(
        this IEnumerable<IGrouping<int, Apartamento>> groupByQuadra)
    {
        int quadraMaisProximaDoFinalDaRua = groupByQuadra.Max(q => q.Key);
        return MelhorQuadraToString(quadraMaisProximaDoFinalDaRua);
    }
    
    private static string MelhorQuadraToString(int quadraMaisProximaDoFinalDaRua) => 
        $"QUADRA {quadraMaisProximaDoFinalDaRua}";

    public static string MelhorQuadraToString(this IEnumerable<MelhorQuadraDto> quadrasComMesmaPrioridade) =>
        $"QUADRA {quadrasComMesmaPrioridade.First().Quadra}";
}
