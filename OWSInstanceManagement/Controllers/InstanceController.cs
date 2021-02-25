﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using SimpleInjector;
using OWSData.Models.StoredProcs;
using OWSShared.Interfaces;
using OWSInstanceManagement.Requests.Instance;
using OWSData.Models.Composites;
using OWSData.Repositories.Interfaces;
using OWSShared.Options;

namespace OWSInstanceManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstanceController : Controller
    {
        private readonly Container _container;
        private readonly IInstanceManagementRepository _instanceManagementRepository;
        private readonly ICharactersRepository _charactersRepository;
        private readonly IOptions<APIPathOptions> _owsApiPathConfig;
        private readonly IHeaderCustomerGUID _customerGuid;

        public InstanceController(Container container,
            IInstanceManagementRepository instanceManagementRepository,
            ICharactersRepository charactersRepository,
            IOptions<APIPathOptions> owsApiPathConfig,
            IHeaderCustomerGUID customerGuid)
        {
            _container = container;
            _instanceManagementRepository = instanceManagementRepository;
            _charactersRepository = charactersRepository;
            _owsApiPathConfig = owsApiPathConfig;
            _customerGuid = customerGuid;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            IHeaderCustomerGUID customerGuid = _container.GetInstance<IHeaderCustomerGUID>();

            if (customerGuid.CustomerGUID == Guid.Empty)
            {
                context.Result = new UnauthorizedResult();
            }
        }

        [HttpPost]
        [Route("SpinUpServerInstance")]
        [Produces(typeof(SuccessAndErrorMessage))]
        /*[SwaggerOperation("ByName")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404)]*/
        public async Task<IActionResult> SpinUpServerInstance([FromBody] SpinUpServerInstanceRequest request)
        {
            request.SetData(_owsApiPathConfig, _charactersRepository, _customerGuid);

            return await request.Handle();
        }

        [HttpPost]
        [Route("ShutDownServerInstance")]
        [Produces(typeof(SuccessAndErrorMessage))]
        /*[SwaggerOperation("ByName")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404)]*/
        public async Task<IActionResult> ShutDownServerInstance([FromBody] ShutDownServerInstanceRequest request)
        {
            request.SetData(_owsApiPathConfig, _instanceManagementRepository, _customerGuid);

            return await request.Handle();
        }

        [HttpPost]
        [Route("GetServerToConnectTo")]
        [Produces(typeof(SuccessAndErrorMessage))]
        /*[SwaggerOperation("ByName")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404)]*/
        public async Task<IActionResult> GetServerToConnectToRequest([FromBody] GetServerToConnectToRequest request)
        {
            request.SetData(_charactersRepository, _customerGuid);

            return await request.Handle();
        }

        [HttpPost]
        [Route("SetZoneInstanceStatus")]
        [Produces(typeof(SuccessAndErrorMessage))]
        /*[SwaggerOperation("ByName")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404)]*/
        public async Task<IActionResult> SetZoneInstanceStatusRequest([FromBody] SetZoneInstanceStatusRequest request)
        {
            request.SetData(_instanceManagementRepository, _customerGuid);

            return await request.Handle();
        }
    }
}