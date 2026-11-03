using Microsoft.Toolkit.Uwp.Notifications;
using Toaster.Configuration;

namespace Toaster;

public class ToastController
{
    private readonly ToastContentBuilder? _contentBuilder = new();

    public ToastContentBuilder Apply(Arguments arguments)
    {
        _contentBuilder!.AddText(arguments.Text);

        if (!string.IsNullOrEmpty(arguments.Title))
            _contentBuilder.AddHeader(Guid.NewGuid().ToString(), arguments.Title, new ToastArguments());

        _contentBuilder.SetToastDuration(ToastDuration.Short);

        _contentBuilder.SetToastScenario(ToastScenario.Reminder);









        return _contentBuilder;
    }
}
