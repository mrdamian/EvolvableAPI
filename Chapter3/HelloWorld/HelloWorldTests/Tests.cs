using HelloWorld.Controllers;
using HelloWorld.Models;
using System;
using System.Net;
using System.Net.Http;
using Xunit;

namespace HelloWorldTests
{
    public class Tests
    {
        [Fact]
        public void TestNewGreetingAdd()
        {
            //arrange
            var greetingName = "newgreeting";
            var greetingMessage = "Hello Test!";
            var fakeRequest = new HttpRequestMessage(HttpMethod.Post,
            "http://localhost:9000/api/greeting");
            var greeting = new Greeting
            {
                Name = greetingName,
                Message = greetingMessage
            };
            var service = new GreetingController();
            service.Request = fakeRequest;
            //act
            var response = service.PostGreeting(greeting);
            //assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(new Uri("http://localhost:9000/api/greeting/newgreeting"),
            response.Headers.Location);
        }
    }
}
