exec("./Support_ClientModuleC.cs");
registerClientModule("VIR", 10);

exec("./magic.cs");
exec("./generic.cs");
exec("./SUPER_SECRET_MESSAGES.cs");

if (!isObject(VIR_DialogView)) {
	new GuiControl(VIR_DialogView) {
		profile = GuiDefaultProfile;

		new GuiMouseEventCtrl() {
			profile = GuiDefaultProfile;
			extent = "1280 720";
			shouldCloseParent = 1;
		};
    };
}

function VIR_OpenDialog()
{
	while (VIR_DialogView.getCount() > 1)
		VIR_DialogView.remove(VIR_DialogView.getObject(1));

    Canvas.pushDialog(VIR_DialogView);
}

function VIR_CloseDialog()
{
    Canvas.popDialog(VIR_DialogView);
}

exec("./inventory/init.cs");
exec("./bank.cs");

exec("./core.cs");
