﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.TeamService.WebApi.Features.AwsConsoleLogin
{
    [Route("")]
    [ApiController]
    public class AwsConsoleController : ControllerBase
    {
        private readonly IAwsConsoleLinkBuilder _awsConsoleLinkBuilder;

        public AwsConsoleController(IAwsConsoleLinkBuilder awsConsoleLinkBuilder)
        {
            _awsConsoleLinkBuilder = awsConsoleLinkBuilder;
        }

        [HttpGet("aws/console")]
        public async Task<ActionResult<AWSConsoleLinkResponse>> ConstructLink([FromQuery] string idToken)
        {
            var consoleLink = await _awsConsoleLinkBuilder.GenerateUriForConsole(idToken);
            return new AWSConsoleLinkResponse(consoleLink.AbsoluteUri);
        }
    }

    public interface IAwsConsoleLinkBuilder
    {
        Task<Uri> GenerateUriForConsole(string idToken);
    }


    public class AWSConsoleLinkResponse
    {

        public AWSConsoleLinkResponse(string absoluteUrl)
        {
            AbsoluteUrl = absoluteUrl;
        }
        public string AbsoluteUrl { get; }
    }




}