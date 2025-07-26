using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Dtos.IOLInvestmentAssetTypes;
using Finance.Application.Mapping;
using Finance.Application.Queries.IOLInvestmentAssetTypes;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/iol-investment-asset-type")]
public class IOLInvestmentAssetTypeQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : BasicQueryController<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum, IOLInvestmentAssetTypeDto, GetAllIOLInvestmentAssetTypesQuery, GetIOLInvestmentAssetTypeQuery>(mapper, dispatcher);
