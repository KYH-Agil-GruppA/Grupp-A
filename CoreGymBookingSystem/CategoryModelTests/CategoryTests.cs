using System.Collections.Generic;
using System.Threading.Tasks;
using MainApp.Pages.Member;
using MainApp.ViewModel;
using DAL.DTOs;
using Moq;
using Services.Interfaces;
using Xunit;

public class CategoryTests
{
    [Fact]
    public async Task OnGetAsync_ShouldLoadSessions()
    {
        var mock = new Mock<ISessionService>();

        mock.Setup(s => s.SearchByCategory("Yoga"))
            .ReturnsAsync(new List<SessionsDto>
            {
                new SessionsDto { Id = 1, Title = "Yoga 1", Description = "Desc", Category = "Yoga" }
            });

        var model = new CategoryModel(mock.Object)
        {
            SelectedCategory = "Yoga"
        };

        await model.OnGetAsync();

        Assert.Single(model.SearchbyCategory);
        Assert.Equal(1, model.SearchbyCategory[0].Id);
        Assert.Equal("Yoga 1", model.SearchbyCategory[0].Title);
    }

    [Fact]
    public async Task OnGetAsync_ShouldReturnEmptyList_WhenNoResults()
    {
        var mock = new Mock<ISessionService>();

        mock.Setup(s => s.SearchByCategory("Nothing"))
            .ReturnsAsync(new List<SessionsDto>());

        var model = new CategoryModel(mock.Object)
        {
            SelectedCategory = "Nothing"
        };

        await model.OnGetAsync();

        Assert.Empty(model.SearchbyCategory);
    }

    [Fact]
    public async Task OnPostAsync_ShouldCallSameLogicAsGet()
    {
        var mock = new Mock<ISessionService>();

        mock.Setup(s => s.SearchByCategory("Cardio"))
            .ReturnsAsync(new List<SessionsDto>
            {
                new SessionsDto { Id = 5, Title = "Cardio Blast", Description = "Intense", Category = "Cardio" }
            });

        var model = new CategoryModel(mock.Object)
        {
            SelectedCategory = "Cardio"
        };

        await model.OnPostAsync();

        Assert.Single(model.SearchbyCategory);
        Assert.Equal(5, model.SearchbyCategory[0].Id);
    }
}
