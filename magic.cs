if (!isObject(VIR_MagicBar)) {
	new GuiSwatchCtrl(VIR_MagicBar) {
		profile = GuiDefaultProfile;

		horizSizing = "right";
		vertSizing = "bottom";

		position = "120 120";
		extent = "24 61";
		minExtent = "8 2";
		
		visible = 0;
		color = "0 0 0 0";

		new GuiBitmapCtrl(Vir_MagicBar_Left) {
			profile = GuiDefaultProfile;

			horizSizing = "right";
			vertSizing = "bottom";

			position = "0 0";
			extent = "11 61";
			minExtent = "8 2";

			bitmap = "Add-Ons/Client_Viridian_RPG/img/magic_bar_left";
			wrap = "0";
			lockAspectRatio = "0";
			alignLeft = "0";
			overflowImage = "0";
			keepCached = "0";
		};

		new GuiBitmapCtrl(Vir_MagicBar_Right) {
			profile = GuiDefaultProfile;

			horizSizing = "right";
			vertSizing = "bottom";

			position = "0 0";
			extent = "11 61";
			minExtent = "8 2";

			bitmap = "Add-Ons/Client_Viridian_RPG/img/magic_bar_right";
			wrap = "0";
			lockAspectRatio = "0";
			alignLeft = "0";
			overflowImage = "0";
			keepCached = "0";
		};
	};

	PlayGUI.add(VIR_MagicBar);
}

function clientCmdVIR_SetMagicLimit(%limit) {
	for (%i = VIR_MagicBar.getCount() - 2; %i < %limit; %i++) {
		%bitmap = new GuiBitmapCtrl() {
			profile = GuiDefaultProfile;

			extent = "32 32";
			bitmap = "base/data/shapes/blank";
		};

		VIR_MagicBar.add(%bitmap);
	}

	for (%i = VIR_MagicBar.getCount() - 2; %i > %limit; %i--) {
		VIR_MagicBar.getObject(%i + 1).delete();
	}

	VIR_MagicBar.reflow();

	if (%limit < 1)
		VIR_MagicBar.setVisible(0);
}

function clientCmdVIR_ClearMagic() {
	%count = VIR_MagicBar.getCount();

	for (%i = 2; %i < %count; %i++) {
		VIR_MagicBar.getObject(%i).setBitmap("base/data/shapes/blank");
	}
	
	VIR_MagicBar.setVisible(0);
}

function clientCmdVIR_SetMagic(%index, %element) {
	%count = VIR_MagicBar.getCount();
	%index = getMax(%index, 0) + 2;

	if (%index >= %count) {
		return;
	}

	%bitmap = VIR_MagicBar.getObject(%index);

	if (%element $= "") {
		%path = "base/data/shapes/blank";
	} else {
		%path = "Add-Ons/Client_Viridian_RPG/m/" @ %element;
	}

	%bitmap.setBitmap(%path);

	for (%i = 2; %i < %count; %i++) {
		if (VIR_MagicBar.getObject(%i).bitmap !$= "base/data/shapes/blank") {
			break;
		}
	}

	VIR_MagicBar.setVisible(%i != %count);
}

function VIR_MagicBar::reflow(%this) {
	%count = %this.getCount();
	%limit = %count - 2;

	%res = getRes();

	%rx = getWord(%res, 0);
	%ry = getWord(%res, 1);

	%width = 22 + %limit * 48 + (%limit - 1) * 2;
	%height = 61;

	%x = %rx / 2 - %width / 2;
	%y = %ry - %height - 151 - 16;

	%this.resize(%x, %y, %width, %height);
	%this.getObject(1).resize(%width - 11, 0, 11, 61);

	for (%i = 2; %i < %count; %i++) {
		%index = %i - 2;

		%bitmap = %this.getObject(%i);
		%bitmap.resize(12 + %index * (48 + 2), 7, 48, 48);
	}
}

package VIR_MagicBarPackage {
	function PlayGUI::onRender(%this) {
		Parent::onRender(%this);
		VIR_MagicBar.reflow();
	}
};

activatePackage("VIR_MagicBarPackage");