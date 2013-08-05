using System.Collections.Generic;
using System.Linq;
using AmazingCloudSearch.Builder;
using AmazingCloudSearch.Contract;
using AmazingCloudSearch.Enum;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AmazingCloudSearch.Test
{
    [TestFixture]
    public class SanitizeBadCharsTester
    {
        readonly ActionBuilder<Movie> _actionBuilder = new ActionBuilder<Movie>();

        [Test]
        public void ShouldSanitizeBadChars()
        {
            var movie = new Movie { id = @"fjuhewdijsdjoi¿¶á" };
            var expectedMovie = new Movie { id = "fjuhewdijsdjoi" };

            List<BasicDocumentAction> liAction = new List<BasicDocumentAction>();
            liAction.Add(_actionBuilder.BuildAction(movie, ActionType.ADD));
            var actual = mockPerformDocumentAction(liAction);

            liAction.Clear();
            liAction.Add(_actionBuilder.BuildAction(expectedMovie, ActionType.ADD));
            var expected = mockPerformDocumentAction(liAction);

            getId(actual).ShouldEqual(getId(expected));
        }

        [TestCase(@"Testá", Result = 4)]
        [TestCase(@"Test¶", Result = 4)]
        [TestCase(@"Test¿", Result = 4)]
        [TestCase(@"Test☻", Result = 5)]
        [TestCase(@"Test♠", Result = 5)]
        [TestCase("{\"id\": \"Test¿¶á¿¶á¿¶á¿¶á\"}", Result = 14)]
        public int ValidSanitize(string docText)
        {
            return CloudSearch<Movie>.SanitizeForCloudSearch(docText).Length;
        }

        private string mockPerformDocumentAction(List<BasicDocumentAction> liAction)
        {
            return CloudSearch<Movie>.SanitizeForCloudSearch(JsonConvert.SerializeObject(liAction));
        }

        private string getId(string jsonObj)
        {
            return JsonConvert.DeserializeObject<List<BasicDocumentAction>>(jsonObj).First().id;
        }
    }
}
