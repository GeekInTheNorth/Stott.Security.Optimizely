namespace OptimizelyTwelveTest.Features.Users
{
    using EPiServer.Authorization;
    using EPiServer.Shell.Security;

    using Microsoft.AspNetCore.Mvc;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    [ApiController]
    public class UserConfigController : ControllerBase
    {
        private readonly UIUserProvider _userProvider;
        private readonly UIRoleProvider _roleProvider;

        public UserConfigController(UIUserProvider userProvider, UIRoleProvider roleProvider)
        {
            _userProvider = userProvider;
            _roleProvider = roleProvider;
        }

        [Route("webapi/user/setupadmin")]
        public async Task<IActionResult> SetUpAdmin()
        {
            var adminEmail = "mark.stott@twentysixdigital.com";
            var adminUser = await _userProvider.GetUserAsync(adminEmail);
            if (adminUser == null)
            {
                var result = await _userProvider.CreateUserAsync(adminEmail, "LetMeIn99!", adminEmail, null, null, true);
                var roles = new List<string> {Roles.WebAdmins, Roles.WebEditors, Roles.Administrators};
                foreach (var role in roles)
                {
                    var exists = await _roleProvider.RoleExistsAsync(role);
                    if (!exists)
                    {
                        await _roleProvider.CreateRoleAsync(role);
                    }
                }

                await _roleProvider.AddUserToRolesAsync(result.User.Username, roles);
            }

            return Ok();
        }

        [Route("webapi/user/deleteadmin")]
        public async Task<IActionResult> DeleteAdmin()
        {
            var adminEmail = "mark.stott@twentysixdigital.com";
            await _userProvider.DeleteUserAsync(adminEmail, true);

            return Ok();
        }
    }
}
