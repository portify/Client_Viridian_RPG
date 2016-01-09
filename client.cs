exec("./Support_ClientModuleC.cs");
registerClientModule("VIR", 11);

exec("./magic.cs");
exec("./generic.cs");
exec("./SUPER_SECRET_MESSAGES.cs");

if (!isObject(VIR_DialogView)) {
	new GuiControl(VIR_DialogView) {
		profile = GuiDefaultProfile;

		new GuiMouseEventCtrl() {
			profile = GuiDefaultProfile;
			extent = "1280 720";
			shouldCloseVIRDialog = 1;
		};
  };
}

function VIR_OpenDialog(%type)
{
	echo("vir_opendialog" SPC %type);
	VIR_DialogView.type = %type;

	while (VIR_DialogView.getCount() > 1)
		VIR_DialogView.remove(VIR_DialogView.getObject(1));

    Canvas.pushDialog(VIR_DialogView);
}

function VIR_CloseDialog(%force)
{
	if (!%force)
	{
		echo("close" SPC VIR_DialogView.type);
		switch$ (VIR_DialogView.type)
		{
			case "bank": commandToServer('VIR_CloseBank'); return;
		}
	}

  Canvas.popDialog(VIR_DialogView);
}

exec("./inventory/init.cs");
exec("./bank.cs");

exec("./core.cs");
