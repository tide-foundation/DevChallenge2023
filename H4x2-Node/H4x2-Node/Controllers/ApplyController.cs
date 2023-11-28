﻿// 
// Tide Protocol - Infrastructure for a TRUE Zero-Trust paradigm
// Copyright (C) 2022 Tide Foundation Ltd
// 
// This program is free software and is subject to the terms of 
// the Tide Community Open Code License as published by the 
// Tide Foundation Limited. You may modify it and redistribute 
// it in accordance with and subject to the terms of that License.
// This program is distributed WITHOUT WARRANTY of any kind, 
// including without any implied warranty of MERCHANTABILITY or 
// FITNESS FOR A PARTICULAR PURPOSE.
// See the Tide Community Open Code License for more details.
// You should have received a copy of the Tide Community Open 
// Code License along with this program.
// If not, see https://tide.org/licenses_tcoc2-0-0-en
//


using Microsoft.AspNetCore.Mvc;
using H4x2_TinySDK.Ed25519;
using H4x2_TinySDK.Math;
using System.Numerics;

namespace H4x2_Node.Controllers
{
    public class ApplyController : Controller
    {
        private Settings _settings { get; }
        private ThrottlingManager _throttlingManager;
        public ApplyController(Settings settings)
        {
            _settings = settings;
            _throttlingManager = new ThrottlingManager();
        }

        public ActionResult<string> Prism([FromBody] Point point) => Apply(point, _settings.PRISM);

        private ActionResult<string> Apply(Point toApply, BigInteger key)
        {
            try
            {
                var barredTime = Throttle().Value;
                if (!barredTime.Equals(0)) 
                    return StatusCode(429,barredTime.ToString());
                
                Point appliedPoint = PRISM.Apply(toApply, key);
                return appliedPoint.ToBase64();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        private ActionResult<int> Throttle()
        {
            var Ip = (HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "").Split(new char[] { ':' }).FirstOrDefault();
            if (!String.IsNullOrEmpty(Ip)) 
                return _throttlingManager.Throttle(Ip.ToString()).GetAwaiter().GetResult();
            else
                return _throttlingManager.Throttle(Request.HttpContext.Connection.RemoteIpAddress.ToString()).GetAwaiter().GetResult();
        }
    }
}
