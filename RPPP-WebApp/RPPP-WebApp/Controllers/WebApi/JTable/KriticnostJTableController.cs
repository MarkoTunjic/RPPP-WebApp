using Microsoft.AspNetCore.Mvc;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Models.JTable;

namespace WebApi.Controllers.JTable
{
    [Route("")]
    public class KriticnostJTableController : JTableController<KriticnostJController, int, KriticnostJViewModel>
    {
        public KriticnostJTableController(KriticnostJController controller) : base(controller)
        {

        }

        [HttpPost]
        public async Task<JTableAjaxResult> Update([FromForm] KriticnostJViewModel model)
        {
            return await base.UpdateItem(model.Id, model);
        }

        [HttpPost]
        public async Task<JTableAjaxResult> Delete([FromForm] int Id)
        {
            return await base.DeleteItem(Id);
        }
    }
}