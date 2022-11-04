using System.Collections.Generic;
using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;

namespace ProcurandoApartamento.Domain.Services.Interfaces
{
    public interface IApartamentoService
    {
        Task<Apartamento> Save(Apartamento apartamento);

        Task<IPage<Apartamento>> FindAll(IPageable pageable);

        Task<Apartamento> FindOne(long id);

        Task Delete(long id);
        Task<string> RecuperaMelhorApartamento(List<string> estabelecimentos);
    }
}
