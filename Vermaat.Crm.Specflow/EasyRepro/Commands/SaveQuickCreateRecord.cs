using Microsoft.Dynamics365.UIAutomation.Browser;
using System;
using System.Linq;
using System.Threading;

namespace Vermaat.Crm.Specflow.EasyRepro.Commands
{
    public class SaveQuickCreateRecord : ISeleniumCommand
    {
        private readonly bool _saveIfDuplicate;

        public SaveQuickCreateRecord(bool saveIfDuplicate)
        {
            _saveIfDuplicate = saveIfDuplicate;
        }

        public CommandResult Execute(BrowserInteraction browserInteraction)
        {
            Logger.WriteLine($"Saving Record");
            var saveButton = browserInteraction.Driver.WaitUntilAvailable(browserInteraction.Selectors.GetXPathSeleniumSelector(SeleniumSelectorItems.Entity_QuickCreate_SaveAndCloseButton));
            if(saveButton == null)
                CommandResult.Fail(true, Constants.ErrorCodes.QUICK_CREATE_SAVE_BUTTON_DOESNT_EXIST);

            saveButton.Click(true);
            browserInteraction.Driver.WaitForTransaction();

            var timeout = DateTime.Now.AddSeconds(20);
            bool saveCompleted = false;
            while (!saveCompleted && DateTime.Now < timeout)
            {
                Logger.WriteLine("Save not yet completed. Waiting..");
                Thread.Sleep(200);

                var duplicateDetectionResult = SeleniumCommandProcessor.ExecuteCommand(browserInteraction, browserInteraction.SeleniumCommandFactory.CreateCheckForDuplicateDetection(_saveIfDuplicate));
                switch (duplicateDetectionResult)
                {
                    case DuplicateDetectionResult.DuplicateDetectionAccepted:
                        continue;
                    case DuplicateDetectionResult.DuplicateDetectionRejected:
                        return CommandResult.Fail(false, Constants.ErrorCodes.FORM_SAVE_FAILED, $"Duplicate detected and saving with duplicate is not allowed");
                    case DuplicateDetectionResult.NoDuplicateDetection:
                    default:
                    {
                        if (browserInteraction.Driver.HasElement(browserInteraction.Selectors.GetXPathSeleniumSelector(SeleniumSelectorItems.Entity_QuickCreate_Notification_Window)))
                        {
                            saveCompleted = true;
                        }
                        else
                        {
                            var formNotifications = SeleniumCommandProcessor.ExecuteCommand(browserInteraction, browserInteraction.SeleniumCommandFactory.CreateGetFormNotificationsCommand());
                            if (formNotifications.Any())
                            {
                                throw new TestExecutionException(Constants.ErrorCodes.FORM_SAVE_FAILED, $"Detected Unsaved changes. Form Notifications: {string.Join(", ", formNotifications)}");
                            }
                        }

                        break;
                    }
                }
            }

            if (!saveCompleted)
                return CommandResult.Fail(false, Constants.ErrorCodes.FORM_SAVE_TIMEOUT, 20);

            return CommandResult.Success();
        }
    }
}
