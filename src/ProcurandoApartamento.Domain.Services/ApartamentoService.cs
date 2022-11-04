using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using ProcurandoApartamento.Domain.Services.Interfaces;
using ProcurandoApartamento.Domain.Repositories.Interfaces;
using ProcurandoApartamento.Dto;
using static ProcurandoApartamento.Domain.Services.QuadraService;

namespace ProcurandoApartamento.Domain.Services
{
    public class ApartamentoService : IApartamentoService
    {
        protected readonly IApartamentoRepository _apartamentoRepository;

        public ApartamentoService(IApartamentoRepository apartamentoRepository)
        {
            _apartamentoRepository = apartamentoRepository;
        }

        public virtual async Task<Apartamento> Save(Apartamento apartamento)
        {
            await _apartamentoRepository.CreateOrUpdateAsync(apartamento);
            await _apartamentoRepository.SaveChangesAsync();
            return apartamento;
        }

        public virtual async Task<IPage<Apartamento>> FindAll(IPageable pageable)
        {
            var page = await _apartamentoRepository.QueryHelper()
                .GetPageAsync(pageable);
            return page;
        }

        public virtual async Task<Apartamento> FindOne(long id)
        {
            var result = await _apartamentoRepository.QueryHelper()
                .GetOneAsync(apartamento => apartamento.Id == id);
            return result;
        }

        public virtual async Task Delete(long id)
        {
            await _apartamentoRepository.DeleteByIdAsync(id);
            await _apartamentoRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Método responsável por recupearar a melhor quadra para se alugar determinado apartamento
        /// De acordo com a disponibilidade e lista de estabelecimentos desejados pelo usuário
        /// </summary>
        /// <param name="estabelecimentosDesejados"></param>
        /// <returns></returns>
        public async Task<string> RecuperaMelhorApartamento(List<string> estabelecimentosDesejados)
        {
            var todosApartamentos = await _apartamentoRepository.GetAllAsync();
            var quadras = todosApartamentos.GroupBy(ta => ta.Quadra).ToList();
            var listaDasMelhoresQuadras = new List<MelhorQuadraDto>();
            
            foreach (var quadra in quadras)
                ValidaSeExisteEstabelecimentoNaQuadra(estabelecimentosDesejados, quadra, listaDasMelhoresQuadras);
            
            if (listaDasMelhoresQuadras.Count == 0) return quadras.ToList().RecuperaQuadraMaisProximaDoFimDaRua();

            List<MelhorQuadraDto> quadrasComMesmaQuantidadeDeEstabelecimentos = 
                listaDasMelhoresQuadras.ExisteQuadrasComMesmaQuantidadeDeEstabelecimentos();
            if (quadrasComMesmaQuantidadeDeEstabelecimentos.Count() == 1) 
                return quadrasComMesmaQuantidadeDeEstabelecimentos.MelhorQuadraToString();

            List<MelhorQuadraDto> quadrasComMesmaPrioridade = 
                quadrasComMesmaQuantidadeDeEstabelecimentos.ExisteQuadrasComMesmaPrioridade();
            return quadrasComMesmaPrioridade.Count == 1 ? 
                quadrasComMesmaPrioridade.MelhorQuadraToString() : quadrasComMesmaPrioridade.RecuperaQuadraMaisProximaDoFimDaRua();
        }
    }
}
