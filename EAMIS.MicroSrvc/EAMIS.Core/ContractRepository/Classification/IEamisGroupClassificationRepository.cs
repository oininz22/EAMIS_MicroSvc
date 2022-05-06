﻿using EAMIS.Common.DTO.Classification;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Classification
{
    public interface IEamisGroupClassificationRepository
    {
        Task<DataList<EamisGroupClassificationDTO>> List(EamisGroupClassificationDTO filter, PageConfig config);
        Task<EamisGroupClassificationDTO> Insert(EamisGroupClassificationDTO item);
        Task<EamisGroupClassificationDTO> Update(EamisGroupClassificationDTO item, int Id);
        Task<EamisGroupClassificationDTO> Delete(EamisGroupClassificationDTO item, int Id);
    }
}
