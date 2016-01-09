new GuiControlProfile(VIR_ScrollProfile : GuiScrollProfile)
{
    bitmap = "./img/scrollFull.png";

    borderColor = "0 0 0 0";
    borderThickness = 0;
    fillColor = "71 71 71";
};

if(!isObject(VIR_bank))
    exec("./ui/VIR_Bank.gui");

function VIR_TabClick(%this)
{
    %group = %this.getGroup();

    for(%i = 0; %i < %group.getCount(); %i++)
        %group.getObject(%i).setBitmap("Add-Ons/Client_Viridian_RPG/img/tab");

    %this.setBitmap("Add-Ons/Client_Viridian_RPG/img/tabUse");

    // Change scroll area for each tab
}

function clientCmdVIR_OpenBank()
{
  VIR_OpenDialog("bank");
  VIR_DialogView.add(VIR_Inventory);
  VIR_DialogView.add(VIR_Bank);

  %res = getRes();
  %cx = getWord(%res, 0) / 2;
  %cy = getWord(%res, 1) / 2;

  VIR_Inventory.position = %cx + 8 SPC %cy - getWord(VIR_Inventory.extent, 1) / 2;
  VIR_Bank.position = %cx - getWord(VIR_Bank.extent, 0) - 8 SPC %cy - getWord(VIR_Bank.extent, 1) / 2;
}

function clientCmdVIR_CloseBank()
{
  VIR_CloseDialog(true);
}

function VIR_clickBankSlot(%button)
{
  %slot = %button.getGroup();

  if (!isObject(%slot.itemData))
    return;

  commandToServer('VIR_TransferItem', %slot.itemData, 1, 'bank', 'inventory');
}

function VIR_altClickBankSlot(%button)
{
  %slot = %button.getGroup();

  if (!isObject(%slot.itemData))
    return;

  VIR_InventoryDropPopup.slot = %slot;
  VIR_InventoryDropPopup.withdrawing = true;
  VIR_InventoryDropPopupBack.position = Canvas.getCursorPos();
  VIR_InventoryDropPopupInput.setValue("");
  VIR_InventoryDropPopupInput.makeFirstResponder(1);

  VIR_InventoryDropBtnCnt.setText("Withdraw");
  VIR_InventoryDropBtnAll.setText("All");
  VIR_InventoryDropBtnCnt.resize( 76, 8, 55, 18);
  VIR_InventoryDropBtnAll.resize(135, 8, 35, 18);

  Canvas.pushDialog(VIR_InventoryDropPopup);
}
