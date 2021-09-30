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
    public class UserTests
    {
        private ITestOutputHelper output;
        public UserTests(ITestOutputHelper helper) => output = helper;
        private UserService userService = new UserService();

        [Fact]
        public async void TestRegister()
        {
            bool registered = await userService.RegisterAsync("fxc@gmail.com", "Password?1", "Password?1");
            Assert.True(registered);

        }
        [Fact]
        public void TestLogin()
        {

        }
    }
}
