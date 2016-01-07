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
    // echo("Hi" SPC %this);

    %group = %this.getGroup();

    for(%i = 0; %i < %group.getCount(); %i++)
    {
        %group.getObject(%i).setBitmap("Add-Ons/Client_Viridian_RPG/img/tab");
    }

    %this.setBitmap("Add-Ons/Client_Viridian_RPG/img/tabUse");

    // Change scroll area for each tab
}

function VIR_RecieveBank()
{

}

function clientCmdVIR_OpenBank()
{
	VIR_OpenDialog();
	VIR_DialogView.add(VIR_Inventory);
	VIR_DialogView.add(VIR_Bank);

    %res = getRes();
    %cx = getWord(%res, 0) / 2;
    %cy = getWord(%res, 1) / 2;

    VIR_Inventory.position = %cx + 8 SPC %cy - getWord(VIR_Inventory.extent, 1) / 2;
    VIR_Bank.position = %cx - getWord(VIR_Bank.extent, 0) - 8 SPC %cy - getWord(VIR_Bank.extent, 1) / 2;
}
