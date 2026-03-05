using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.IOLInvestmentAssetTypes;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Legacy.Queries.IOLInvestmentAssetTypes;
using Finance.Domain.Enums;
using Finance.Domain.Models.IOLInvestments;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/iol-investment-asset-type")]
public class IOLInvestmentAssetTypeQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : BasicQueryController<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum, IOLInvestmentAssetTypeDto, GetAllIOLInvestmentAssetTypesQuery, GetIOLInvestmentAssetTypeQuery>(mapper, dispatcher);
