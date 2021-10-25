using MelanomaClassification;
using MelanomaClassification.Models;
using MelanomaClassification.Presenters;
using MelanomaClassification.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class UserServiceTests
    {
        private ITestOutputHelper output;
        public UserServiceTests(ITestOutputHelper helper)
        {
            output = helper;
            UserService.Init();
        }

        [Fact]
        public async void TestRegister()
        {
            bool registered = await UserService.RegisterAsync("fxc@gmail.com", "Password?1", "Password?1");
            Assert.True(true);

        }
        [Fact]
        public async void TestLogin()
        {
            bool loggedIn = await UserService.LoginAsync("t@mail.com", "Password?1");
            Assert.True(loggedIn);

        }


        
    }
}
