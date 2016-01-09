$mod_lshift = 1 << 0;
$mod_rshift = 1 << 1;
$mod_lctrl  = 1 << 2;
$mod_rctrl  = 1 << 3;
$mod_lalt   = 1 << 4;
$mod_ralt   = 1 << 5;

if (!isFunction("GuiMouseEventCtrl", "onMouseDown")) {
	eval("function GuiMouseEventCtrl::onMouseDown(){}");
}

if (!isFunction("GuiMouseEventCtrl", "onMouseUp")) {
	eval("function GuiMouseEventCtrl::onMouseUp(){}");
}

package VIR_InventoryPacman {
	function GuiMouseEventCtrl::onMouseDown(%this, %mod, %pos, %state) {
		Parent::onMouseDown(%this, %mod, %pos, %state);

		if (%this.shouldCloseVIRDialog)
		{
			VIR_CloseDialog();
			return;
		}

		if (%this.shouldCloseParent)
		{
			// this probably shouldn't be here anymore
			Canvas.popDialog(%this.getGroup());
			return;
		}

		%slot = %this.getGroup();

		if (isObject(%slot.itemData)) {
			VIR_InventoryPopup.actualSlot = %slot;
			VIR_InventoryPopup.slot = %slot.itemSlot;
			VIR_InventoryPopup.data = %slot.itemData;
			VIR_InventoryPopupBack.position = %pos;

			if (%mod & ($mod_lalt | $mod_ralt)) {
				VIR_InventoryPopupText.onURL("gamelink_drop");
			} else if (%mod & ($mod_lshift | $mod_rshift)) {
				if (%slot.itemData.actions !$= "") {
					VIR_InventoryPopupText.onURL("gamelink_0");
				}
			} else {
				%text = "<color:ffffff><font:arial bold:18>" @ %slot.itemData.uiName;
				%text = %text @ "<font:arial bold:16><linkcolor:ffaa33>";

				if (!VIR_Bank.isAwake())
				{
					if (%slot.itemData.actions !$= "") {
						%count = getLineCount(%slot.itemData.actions);

						for (%i = 0; %i < %count; %i++) {
							%line = getLine(%slot.itemData.actions, %i);
							%text = %text @ "\n<a:gamelink_" @ %i @ ">" @ getField(%line, 0) @ "</a>";
						}
					}

					if (%slot.itemData.equipSlot !$= "") {
						if (%slot.slotEquip) {
							%text = %text @ "\n<a:gamelink_unequip>Unequip</a>";
						} else {
							%text = %text @ "\n<a:gamelink_equip>Equip</a>";
						}
					}

					if (%slot.item.data.equipSlot $= "" || !%slot.slotEquip)
						%text = %text @ "\n<a:gamelink_drop>Drop</a>";
				}
				else
				{
					%text = %text @ "\n<a:gamelink_drop>Deposit</a>";
				}

				%text = %text @ "\n<a:gamelink_cancel>Cancel</a>";

				if (%slot.itemData.description !$= "") {
					%text = %text @ "\n<font:arial:14>" @ %slot.itemData.description;
				}

				Canvas.pushDialog(VIR_InventoryPopup);

				VIR_InventoryPopupText.setValue(%text);
				VIR_InventoryPopupText.forceReflow();

				VIR_InventoryPopupBack.resize(
					getWord(%pos, 0), getWord(%pos, 1),
					getWord(VIR_InventoryPopupBack.extent, 0),
					getWord(VIR_InventoryPopupText.extent, 1) + 4);
			}
		}
	}

	function GuiMouseEventCtrl::onMouseUp(%this, %mod, %pos, %state) {
		Parent::onMouseUp(%this, %mod, %pos, %state);
	}
};

activatePackage("VIR_InventoryPacman");

function GuiControl::findByInternalName(%this, %internalName, %immediate) {
	%count = %this.getCount();

	for (%i = 0; %i < %count; %i++) {
		%child = %this.getObject(%i);

		if (%child.internalName $= %internalName) {
			return %child;
		}

		if (!%immediate) {
			%search = %child.findByInternalName(%internalName);

			if (isObject(%search)) {
				return %search;
			}
		}
	}

	return 0;
}

function VIR_InventoryPopupText::onURL(%this, %url) {
	%action = getSubStr(%url, 9, strlen(%url));

	if ((VIR_Bank.isAwake() && %action !$= "cancel") || (%action $= "drop" && VIR_InventoryPopup.actualSlot.itemCount > 1)) {
		VIR_InventoryDropPopup.slot = VIR_InventoryPopup.actualSlot;
		VIR_InventoryDropPopup.withdrawing = false;
		VIR_InventoryDropPopupBack.position = VIR_InventoryPopupBack.position;
		VIR_InventoryDropPopupInput.setValue("");
		VIR_InventoryDropPopupInput.makeFirstResponder(1);

		if (VIR_Bank.isAwake())
		{
			VIR_InventoryDropBtnCnt.setText("Deposit");
			VIR_InventoryDropBtnAll.setText("All");
			VIR_InventoryDropBtnCnt.resize( 76, 8, 55, 18);
			VIR_InventoryDropBtnAll.resize(135, 8, 35, 18);
		}
		else
		{
			VIR_InventoryDropBtnCnt.setText("Drop");
			VIR_InventoryDropBtnAll.setText("Drop All");
			VIR_InventoryDropBtnCnt.resize( 76, 8, 40, 18);
			VIR_InventoryDropBtnAll.resize(120, 8, 50, 18);
		}

		Canvas.pushDialog(VIR_InventoryDropPopup);
		Canvas.popDialog(VIR_InventoryPopup);
	} else if (%action $= "equip") {
		commandToServer('VIR_InventoryEquip', VIR_InventoryPopup.slot);
	} else if (%action $= "unequip") {
		commandToServer('VIR_InventoryUnEquip', VIR_InventoryPopup.data.equipSlot);
	} else if (%action !$= "cancel") {
		commandToServer('VIR_InventoryAction', VIR_InventoryPopup.slot, %action);

		if (%action !$= "drop" && getField(getLine(VIR_InventoryPopup.data.actions, %action), 2)) {
			Canvas.popDialog(VIR_InventoryPopup);
			Canvas.popDialog(VIR_DialogView);
		}
	}

	Canvas.popDialog(VIR_InventoryPopup);
}

function clientCmdVIR_SetInventory(%scope, %index, %data, %count) {
	%scope = getTaggedString(%scope);

	if (%scope $= "equipment") {
		%slot = VIR_Inventory.getObject(2).findByInternalName(%index, 1);
	} else if (%scope $= "items") {
		%slot = VIR_Inventory.getObject(3).getObject(%index);
	} else if (%scope $= "bank") {
		%slot = VIR_Bank.getObject(0).getObject(0).getObject(%index);
	} else {
		return;
	}

	if (!isObject(%slot))
		return;

	%slot.itemData = %data;
	%slot.itemCount = %count;

	if (isObject(%data)) {
		if (%data.iconName !$= "" && isFile(%data.iconName)) {
			%slot.getObject(0).setBitmap(%data.iconName);
		} else {
			%slot.getObject(0).setBitmap("Add-Ons/Client_Viridian_RPG/img/unknown");
		}

		if (%count > 1) {
			if (%count > 10000000) {
				%show = "<shadowcolor:003300><color:00bb00>" @ ((%count / 1000000) | 0) @ "mil";
			} else if (%count > 100000) {
				%show = "<shadowcolor:0000ff><color:00ffff>" @ ((%count / 1000) | 0) @ "k";
			} else {
				%show = "<shadowcolor:000000><color:ffffff>" @ %count;
			}

			%slot.getObject(1).setValue("<just:right><shadow:1:1><font:arial bold:14>" @ %show);
		} else {
			%slot.getObject(1).setValue("");
		}
	} else {
		%slot.getObject(0).setBitmap("base/data/shapes/blank");
		%slot.getObject(1).setValue("");
	}
}

function clientCmdVIR_ClearInventory() {
	clientCmdVIR_SetInventory('equipment', "slot1", 0, 0);
	clientCmdVIR_SetInventory('equipment', "slot2", 0, 0);
	clientCmdVIR_SetInventory('equipment', "slot3", 0, 0);
	clientCmdVIR_SetInventory('equipment', "slot4", 0, 0);
	clientCmdVIR_SetInventory('equipment', "head", 0, 0);
	clientCmdVIR_SetInventory('equipment', "chest", 0, 0);
	clientCmdVIR_SetInventory('equipment', "legs", 0, 0);
	clientCmdVIR_SetInventory('equipment', "misc", 0, 0);

	for (%i = 0; %i < 36; %i++) {
		clientCmdVIR_SetInventory('items', %i, 0, 0);
	}

	for (%i = 0; %i < 72; %i++) {
		clientCmdVIR_SetInventory('bank', %i, 0, 0);
	}
}

function clientCmdVIR_SetContainer(%scope, %slot, %item, %count)
{
	clientCmdVIR_SetInventory(%scope, %slot, %item, %count);
}

function clientCmdVIR_ClearContainer(%scope)
{
	switch$ (getTaggedString(%scope))
	{
		case "items": %slots = 36;
		case "bank": %slots = 72;
	}

	for (%i = 0; %i < %slots; %i++)
		clientCmdVIR_SetContainer(%scope, %i);
}

function clientCmdVIR_ItemData(%item, %attr, %value)
{
	switch (stripos("_abcdefghijklmnopqrstuvwxyz", getSubStr(%attr, 0, 1)))
	{
      case  0: %item._[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case  1: %item.a[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case  2: %item.b[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case  3: %item.c[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case  4: %item.d[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case  5: %item.e[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case  6: %item.f[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case  7: %item.g[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case  8: %item.h[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case  9: %item.i[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 10: %item.j[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 11: %item.k[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 12: %item.l[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 13: %item.m[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 14: %item.n[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 15: %item.o[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 16: %item.p[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 17: %item.q[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 18: %item.r[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 19: %item.s[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 20: %item.t[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 21: %item.u[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 22: %item.v[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 23: %item.w[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 24: %item.x[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 25: %item.y[getSubStr(%attr, 1, strlen(%attr))] = %value;
      case 26: %item.z[getSubStr(%attr, 1, strlen(%attr))] = %value;
	}
}

function VIR_Inventory_DropItem(%all)
{
	Canvas.popDialog(VIR_InventoryDropPopup);

	if (%all) {
		%count = VIR_InventoryDropPopup.slot.itemCount;

		if (%count $= "")
			%count = 1;
	} else {
		%count = VIR_InventoryDropPopupInput.getValue();

		if (%count $= "") {
			%count = 1;
		} else if (strlen(%count) > 3 && getSubStr(%count, strlen(%count) - 3, 3) $= "mil") {
			%count = (getSubStr(%count, 0, strlen(%count) - 3) * 1000000) | 0;
		} else if (strlen(%count) > 1 && getSubStr(%count, strlen(%count) - 1, 1) $= "m") {
			%count = (getSubStr(%count, 0, strlen(%count) - 1) * 1000000) | 0;
		} else if (strlen(%count) > 1 && getSubStr(%count, strlen(%count) - 1, 1) $= "k") {
			%count = (getSubStr(%count, 0, strlen(%count) - 1) * 1000) | 0;
		}
	}

	if (VIR_Bank.isAwake())
	{
		if (VIR_InventoryDropPopup.withdrawing)
		{
			%source = 'bank';
			%target = 'inventory';
		}
		else
		{
			%source = 'inventory';
			%target = 'bank';
		}

		commandToServer('VIR_TransferItem', VIR_InventoryDropPopup.slot.itemData, %count, %source, %target);
	}
	else
		commandToServer('VIR_DropItem', VIR_InventoryDropPopup.slot.itemData, %count);
}

if (!isObject(VIR_InventoryDropPopup)) {
	new GuiControl(VIR_InventoryDropPopup) {
		profile = "GuiDefaultProfile";

		new GuiMouseEventCtrl() {
			profile = "GuiDefaultProfile";
			extent = "1280 720";
			shouldCloseParent = 1;
		};

		new GuiSwatchCtrl(VIR_InventoryDropPopupBack) {
			profile = "GuiDefaultProfile";
			extent = "227 34";
			color = "50 50 50 255";

			new GuiTextEditCtrl(VIR_InventoryDropPopupInput) {
				position = "8 8";
				extent = "60 18";
				altCommand = "VIR_Inventory_DropItem(0);";
			};

			new GuiBitmapButtonCtrl(VIR_InventoryDropBtnCnt) {
				profile = GuiCenterTextProfile;
				bitmap = "Add-Ons/Client_Viridian_RPG/img/small_button";
				position = "76 8";
				extent = "40 18";
				text = "Drop";
				command = "VIR_Inventory_DropItem(0);";
			};

			new GuiBitmapButtonCtrl(VIR_InventoryDropBtnAll) {
				profile = GuiCenterTextProfile;
				bitmap = "Add-Ons/Client_Viridian_RPG/img/small_button";
				position = "120 8";
				extent = "50 18";
				text = "Drop All";
				command = "VIR_Inventory_DropItem(1);";
			};

			new GuiBitmapButtonCtrl() {
				profile = GuiCenterTextProfile;
				bitmap = "Add-Ons/Client_Viridian_RPG/img/small_button";
				position = "174 8";
				extent = "45 18";
				text = "Cancel";
				command = "Canvas.popDialog(VIR_InventoryDropPopup);";
			};
		};
	};
}

if (!isObject(VIR_InventoryPopup)) {
	new GuiControl(VIR_InventoryPopup) {
		profile = "GuiDefaultProfile";
		new GuiMouseEventCtrl() {
			profile = "GuiDefaultProfile";
			extent = "1280 720";
			shouldCloseParent = 1;
		};

		new GuiSwatchCtrl(VIR_InventoryPopupBack) {
			profile = "GuiDefaultProfile";
			extent = "164 0";
			color = "50 50 50 255";

			new GuiMLTextCtrl(VIR_InventoryPopupText) {
				profile = "GuiDefaultProfile";
				position = "2 2";
				extent = "160 0";
				selectable = 0;
			};
		};
	};
}

function VIR_createInventorySlots()
{
	VIR_Inventory.getObject(3).deleteAll();

	for (%i = 0; %i < 36; %i++) {
		%entry = new GuiSwatchCtrl() {
			profile = "GuiDefaultProfile";
			position = 4 + (%i % 9) * 52 SPC 4 + mFloor(%i / 9) * 52;
			extent = "48 48";
			color = "255 255 255 15";

			itemSlot = %i;

			new GuiBitmapCtrl() {
				profile = "GuiDefaultProfile";
				bitmap = "base/data/shapes/blank";
				extent = "48 48";
			};

			new GuiMLTextCtrl() {
				profile = "GuiDefaultProfile";
				position = "0 34";
				extent = "46 14";
			};

			new GuiMouseEventCtrl() {
				profile = "GuiDefaultProfile";
				extent = "48 48";
			};
		};

		VIR_Inventory.getObject(3).add(%entry);
	}
}

if (!isObject(VIR_Inventory)) {
	// new GuiControl(VIR_Inventory) {
	// 	profile = "GuiDefaultProfile";
	// 	new GuiMouseEventCtrl() {
	// 		profile = "GuiDefaultProfile";
	// 		extent = "1280 720";
	// 		shouldCloseParent = 1;
	// 	};

	new GuiBitmapCtrl(VIR_Inventory) {
		profile = "GuiDefaultProfile";
		horizSizing = "center";
		vertSizing = "center";
		extent = "488 472";
		bitmap = "./panel-main-new";

		new GuiMLTextCtrl() {
			profile = "GuiDefaultProfile";
			position = "10 9";
			extent = "472 24";
			// text = "<color:665533aa><font:arial bold:20>Inventory / Equipment";
			text = "<color:888888aa><font:arial bold:20>Inventory / Equipment";
			selectable = 0;
		};

		new GuiBitmapButtonCtrl() {
			profile = "GuiDefaultProfile";
			position = "462 10";
			extent = "16 15";
			bitmap = "./iconCross";
			text = "";
			command = "VIR_CloseBank();";
			accelerator = "escape"; // does this even work
		};

		new GuiBitmapCtrl() {
			profile = "GuiDefaultProfile";
			bitmap = "./panel-items-new";
			position = "8 32";
			extent = "472 212";

			// ---------
			// Stuff

			// Shoulders
			new GuiSwatchCtrl() { // lazy
				internalName = "shoulders";
				slotEquip = 1;
				color = "255 255 255 15";
				position = "32 24";
				extent = "48 48";

				new GuiBitmapCtrl() {
					bitmap = "base/data/shapes/blank";
					extent = "48 48";
				};

				new GuiMLTextCtrl() {
					position = "0 34";
					extent = "46 14";
				};

				new GuiMouseEventCtrl() {
					extent = "48 48";
				};
			};
			new GuiMLTextCtrl() {
				position = "6 76";
				extent = "100 20";
				selectable = 0;
				text = "<just:center><color:ffffff33><font:arial bold:20>SHOULDER";
			};

			// Gloves
			new GuiSwatchCtrl() {
				internalName = "gloves";
				slotEquip = 1;
				color = "255 255 255 15";
				position = "32 120";
				extent = "48 48";

				new GuiBitmapCtrl() {
					bitmap = "base/data/shapes/blank";
					extent = "48 48";
				};

				new GuiMLTextCtrl() {
					position = "0 34";
					extent = "46 14";
				};

				new GuiMouseEventCtrl() {
					extent = "48 48";
				};
			};
			new GuiMLTextCtrl() {
				position = "6 172";
				extent = "100 20";
				selectable = 0;
				text = "<just:center><color:ffffff33><font:arial bold:20>GLOVES";
			};

			// Head
			new GuiSwatchCtrl() {
				internalName = "head";
				slotEquip = 1;
				color = "255 255 255 15";
				position = "132 24";
				extent = "48 48";

				new GuiBitmapCtrl() {
					bitmap = "base/data/shapes/blank";
					extent = "48 48";
				};

				new GuiMLTextCtrl() {
					position = "0 34";
					extent = "46 14";
				};

				new GuiMouseEventCtrl() {
					extent = "48 48";
				};
			};
			new GuiMLTextCtrl() {
				position = "106 76";
				extent = "100 20";
				selectable = 0;
				text = "<just:center><color:ffffff33><font:arial bold:20>HEAD";
			};

			// Chest
			new GuiSwatchCtrl() {
				internalName = "chest";
				slotEquip = 1;
				color = "255 255 255 20";
				position = "132 120";
				extent = "48 48";

				new GuiBitmapCtrl() {
					bitmap = "base/data/shapes/blank";
					extent = "48 48";
				};

				new GuiMLTextCtrl() {
					position = "0 34";
					extent = "46 14";
				};

				new GuiMouseEventCtrl() {
					extent = "48 48";
				};
			};
			new GuiMLTextCtrl() {
				position = "106 172";
				extent = "100 20";
				selectable = 0;
				text = "<just:center><color:ffffff33><font:arial bold:20>CHEST";
			};

			// Back
			new GuiSwatchCtrl() {
				internalName = "misc";
				slotEquip = 1;
				color = "255 255 255 20";
				position = "232 24";
				extent = "48 48";

				new GuiBitmapCtrl() {
					bitmap = "base/data/shapes/blank";
					extent = "48 48";
				};

				new GuiMLTextCtrl() {
					position = "0 34";
					extent = "46 14";
				};

				new GuiMouseEventCtrl() {
					extent = "48 48";
				};
			};
			new GuiMLTextCtrl() {
				position = "206 76";
				extent = "100 20";
				selectable = 0;
				text = "<just:center><color:ffffff33><font:arial bold:20>BACK";
			};

			// Legs
			new GuiSwatchCtrl() {
				internalName = "legs";
				slotEquip = 1;
				color = "255 255 255 20";
				position = "232 120";
				extent = "48 48";

				new GuiBitmapCtrl() {
					bitmap = "base/data/shapes/blank";
					extent = "48 48";
				};

				new GuiMLTextCtrl() {
					position = "0 34";
					extent = "46 14";
				};

				new GuiMouseEventCtrl() {
					extent = "48 48";
				};
			};
			new GuiMLTextCtrl() {
				position = "206 172";
				extent = "100 20";
				selectable = 0;
				text = "<just:center><color:ffffff33><font:arial bold:20>FEET";
			};

			// Amulet
			new GuiSwatchCtrl() {
				internalName = "amulet";
				slotEquip = 1;
				color = "255 255 255 20";
				position = "332 24";
				extent = "48 48";

				new GuiBitmapCtrl() {
					bitmap = "base/data/shapes/blank";
					extent = "48 48";
				};

				new GuiMLTextCtrl() {
					position = "0 34";
					extent = "46 14";
				};

				new GuiMouseEventCtrl() {
					extent = "48 48";
				};
			};
			new GuiMLTextCtrl() {
				position = "306 76";
				extent = "100 20";
				selectable = 0;
				text = "<just:center><color:ffffff33><font:arial bold:20>AMULET";
			};

			// ---------------
			// Equip slots

			// // Slot 1
			// new GuiSwatchCtrl() {
			// 	internalName = "slot1";
			// 	color = "255 255 255 20";
			// 	position = "314 24";
			// 	extent = "48 48";

			// 	new GuiBitmapCtrl() {
			// 		bitmap = "base/data/shapes/blank";
			// 		extent = "48 48";
			// 	};

			// 	new GuiMLTextCtrl() {
			// 		position = "0 34";
			// 		extent = "46 14";
			// 	};

			// 	new GuiMouseEventCtrl() {
			// 		extent = "48 48";
			// 	};
			// };
			// new GuiMLTextCtrl() {
			// 	position = "290 76";
			// 	extent = "100 20";
			// 	selectable = 0;
			// 	text = "<just:center><color:ffffff33><font:arial bold:20>SLOT 1";
			// };

			// // Slot 3
			// new GuiSwatchCtrl() {
			// 	internalName = "slot3";
			// 	color = "255 255 255 20";
			// 	position = "314 120";
			// 	extent = "48 48";

			// 	new GuiBitmapCtrl() {
			// 		bitmap = "base/data/shapes/blank";
			// 		extent = "48 48";
			// 	};

			// 	new GuiMLTextCtrl() {
			// 		position = "0 34";
			// 		extent = "46 14";
			// 	};

			// 	new GuiMouseEventCtrl() {
			// 		extent = "48 48";
			// 	};
			// };
			// new GuiMLTextCtrl() {
			// 	position = "290 172";
			// 	extent = "100 20";
			// 	selectable = 0;
			// 	text = "<just:center><color:ffffff33><font:arial bold:20>SLOT 3";
			// };

			// // Slot 2
			// new GuiSwatchCtrl() {
			// 	internalName = "slot2";
			// 	color = "255 255 255 20";
			// 	position = "392 24";
			// 	extent = "48 48";

			// 	new GuiBitmapCtrl() {
			// 		bitmap = "base/data/shapes/blank";
			// 		extent = "48 48";
			// 	};

			// 	new GuiMLTextCtrl() {
			// 		position = "0 34";
			// 		extent = "46 14";
			// 	};

			// 	new GuiMouseEventCtrl() {
			// 		extent = "48 48";
			// 	};
			// };
			// new GuiMLTextCtrl() {
			// 	position = "366 76";
			// 	extent = "100 20";
			// 	selectable = 0;
			// 	text = "<just:center><color:ffffff33><font:arial bold:20>SLOT 2";
			// };

			// // Slot 4
			// new GuiSwatchCtrl() {
			// 	internalName = "slot4";
			// 	color = "255 255 255 20";
			// 	position = "392 120";
			// 	extent = "48 48";

			// 	new GuiBitmapCtrl() {
			// 		bitmap = "base/data/shapes/blank";
			// 		extent = "48 48";
			// 	};

			// 	new GuiMLTextCtrl() {
			// 		position = "0 34";
			// 		extent = "46 14";
			// 	};

			// 	new GuiMouseEventCtrl() {
			// 		extent = "48 48";
			// 	};
			// };
			// new GuiMLTextCtrl() {
			// 	position = "366 172";
			// 	extent = "100 20";
			// 	selectable = 0;
			// 	text = "<just:center><color:ffffff33><font:arial bold:20>SLOT 4";
			// };
		};

		new GuiBitmapCtrl() {
			bitmap = "./panel-items-new";
			position = "8 252";
			extent = "472 212";
		};
	};

	VIR_createInventorySlots();
}
