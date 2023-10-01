﻿using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.InterfacesBL
{
    public interface IOfficeUI
    {
        Task<ActionResultResponse<OfficeViewModel?>> GetOfficeById(int id);
        Task<ActionResultResponse<DataPage<OfficeViewModel>>> GetOfficePage(OfficeFilterRequest filterRequest);
        Task<ActionResultResponse<string>> Insert(OfficeCreateRequest newOffice);
        Task<ActionResultResponse<string>> Update(UpdateOfficeRequest updatedOffice);
        Task<ActionResultResponse<string>> Delete(long id);
    }
}
