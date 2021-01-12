using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Configuration;
using System.Web;

using unilab.crm.business;
using unilab.crm.factory;
using unilab.crm.domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BFNApi.Models;
using BFNApi.Util;
using System.ServiceModel.Channels;

namespace BFNApi.Controllers.Api
{
    public class RegisterController : ApiController
    {
        public string ProjectCode = ConfigurationManager.AppSettings["ProjectCode"].ToString();
        private unilab.crm.business.interfaces.IDataManagementService dataManagementService;
        public RegisterController()
        {
            dataManagementService = unilab.crm.factory.BusinessDelegateFactory.GetInstance().GetDataManagementManager();
        }
       
        [HttpPost]
        [Route("api/register")]
        public IHttpActionResult Post([FromBody]PostRequestModel model)
        {
            VerifyToken();

            if(!ModelState.IsValid)
            {
                var errorList = ModelState.ToDictionary(
                     kvp => kvp.Key,
                     kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).Where(x => x.Length > 0).ToArray()
                 );
                HttpStatusCode unprocessableEntity = (HttpStatusCode)422;
                return Content(unprocessableEntity, errorList);
            }

            RedemptionSummaryVM outputData = new RedemptionSummaryVM();

            Helper h = new Helper();
            try
            {
                var jsonBody = JsonConvert.SerializeObject(model);
                h.Log(DateTime.Now.ToString("hh:mm:ss tt") + "\t" + "POST" + "\t" + jsonBody.Replace("\r", "").Replace("\t", "").Replace("\n", ""), @"\Access\Access");

                RedemptionSummaryVM Vmodel = new RedemptionSummaryVM();

                Vmodel.account.FirstName = model.firstName;
                Vmodel.account.LastName = model.lastName;
                Vmodel.InputMobileNumber = model.mobileNumber.Replace("+63","");
                Vmodel.account.EmailAddress = model.email;
                Vmodel.account.CityName = model.city;
                Vmodel.specificAnswer.Value = model.respiratoryCondition;
                outputData = dataManagementService.ValidateBFN(Vmodel);

                h.Log(DateTime.Now.ToString("hh:mm:ss tt") + "\t" + "POST" + "\t" + jsonBody.Replace("\r", "").Replace("\t", "").Replace("\n", ""), @"\Logs\Log");
            }
            catch (Exception ex)
            {
                var json = "{";
                json += "\"Status\":\"failed\",";
                json += "\"StatusMessage\":\"" + ex.Message + "\"";
                json += "}";
                h.Log(DateTime.Now.ToString("hh:mm:ss tt") + "RETURN" + "\t" + json.ToString(), @"\Errors\Error");
                h.Log(DateTime.Now.ToString("hh:mm:ss tt") + "RETURN" + "\t" + json.ToString(), @"\Logs\Log");

                HttpStatusCode badRequest = (HttpStatusCode)400;
                return Content(badRequest, json);
            }

            

            return Created(new Uri(Request.RequestUri + "/" + outputData.account.ID), model);
        }

        private void VerifyToken()
        {
            var req = Request;
            var headers = req.Headers;
            Helper h = new Helper();

            if (headers.Contains("token"))
            {
                string token = headers.GetValues("token").First();
                DataTable dt = h.GetTokenByProjectCode(ProjectCode);
                if (dt.Rows.Count > 0)
                {
                    if (token != dt.Rows[0][0].ToString())
                    {
                        h.Log(DateTime.Now.ToString("hh:mm:ss tt") + "\t" + "Forbidden" + "\t" + GetClientIp() + "\t" + token, @"\Access\Access");
                        throw new HttpResponseException(HttpStatusCode.Forbidden);
                    }
                    else
                    {
                        h.Log(DateTime.Now.ToString("hh:mm:ss tt") + "\t" + "Authorized" + "\t" + GetClientIp() + "\t" + token, @"\Access\Access");
                    }
                }
            }
            else
            {
                h.Log(DateTime.Now.ToString("hh:mm:ss tt") + "\t" + "Unauthorized" + "\t" + GetClientIp(), @"\Access\Access");
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }

        private string GetClientIp(HttpRequestMessage request = null)
        {
            request = request ?? Request;

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }

        [HttpOptions]
        public string OptionTest()
        {
            return "true";
        }
    }
}
