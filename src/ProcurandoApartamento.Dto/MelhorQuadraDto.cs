// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this

using System.Collections.Generic;

namespace ProcurandoApartamento.Dto;

public class MelhorQuadraDto
{
    public MelhorQuadraDto()
    {
        ApartamentosDisponiveis = new List<ApartamentoDisponivelDto>();
    }
            
    public void AdicionaApartamentoDisponivel(int prioridade)
    {
        ApartamentosDisponiveis.Add(new ApartamentoDisponivelDto(prioridade));
    }

    public int Quadra { get; init; }

    public List<ApartamentoDisponivelDto> ApartamentosDisponiveis { get; }
}
