﻿using EAMIS.Common.DTO.Classification;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Classification
{
    public interface IEamisSubClassificationRepository
    {
        Task<DataList<EamisSubClassificationDTO>> List(EamisSubClassificationDTO filter, PageConfig config);
        Task<EamisSubClassificationDTO> Insert(EamisSubClassificationDTO item);
        Task<EamisSubClassificationDTO> Update(EamisSubClassificationDTO item, int Id);
        Task<EamisSubClassificationDTO> Delete(EamisSubClassificationDTO item, int Id);
    }
}
