function clientCmdVIR_ConfirmHandshake()
{
	ServerConnection.isVIR = 1;
}

function appendRemap(%name, %command, %binding)
{
	$remapName[$remapCount] = %name;
	$remapCmd[$remapCount] = %command;
	$remapCount++;

	if (%binding !$= "")
		schedule(0, 0, "tryRemap", %command, %binding);
}

function tryRemap(%command, %binding)
{
	%device = getField(%binding, 0);
	%mapping = getField(%binding, 1);

	if (MoveMap.getBinding(%command) $= "" && MoveMap.getCommand(%device, %mapping) $= "")
		MoveMap.bind(%device, %mapping, %command);
}

function toggleDialog(%control) {
	if (Canvas.isMember(%control)) {
		Canvas.popDialog(%control);
	} else {
		Canvas.pushDialog(%control);
	}
}

function VIR_SelfCast(%state) { commandToServer('VIR_SelfCast', %state); }
function VIR_AreaCast(%state) { commandToServer('VIR_AreaCast', %state); }
function VIR_SpecialCast(%state) { commandToServer('VIR_SpecialCast', %state); }
function VIR_ConjureWater(%state)     { if (%state) commandToServer('VIR_Conjure', 0); }
function VIR_ConjureLife(%state)      { if (%state) commandToServer('VIR_Conjure', 1); }
function VIR_ConjureShield(%state)    { if (%state) commandToServer('VIR_Conjure', 2); }
function VIR_ConjureCold(%state)      { if (%state) commandToServer('VIR_Conjure', 3); }
function VIR_ConjureLightning(%state) { if (%state) commandToServer('VIR_Conjure', 4); }
function VIR_ConjureDeath(%state)     { if (%state) commandToServer('VIR_Conjure', 5); }
function VIR_ConjureEarth(%state)     { if (%state) commandToServer('VIR_Conjure', 6); }
function VIR_ConjureFire(%state)      { if (%state) commandToServer('VIR_Conjure', 7); }

function VIR_ToggleInventory(%state)
{
	// Not actually a toggle anymore, just always opens it
	VIR_OpenDialog();
	VIR_DialogView.add(VIR_Inventory);
}

// function VIR_ToggleInventory(%state) { if (%state) toggleDialog(VIR_Inventory); }
function VIR_ToggleCharacter(%state) { if (%state) toggleDialog(VIR_Character); }
function VIR_Ability1(%state) { commandToServer('VIR_Ability', 1, %state); }
function VIR_Ability2(%state) { commandToServer('VIR_Ability', 2, %state); }
function VIR_Ability3(%state) { commandToServer('VIR_Ability', 3, %state); }
function VIR_Ability4(%state) { commandToServer('VIR_Ability', 4, %state); }
function VIR_Ability5(%state) { commandToServer('VIR_Ability', 5, %state); }
function VIR_Ability6(%state) { commandToServer('VIR_Ability', 6, %state); }
function VIR_QuickSlot1(%state) { if (%state) commandToServer('VIR_QuickSlot', 1); }
function VIR_QuickSlot2(%state) { if (%state) commandToServer('VIR_QuickSlot', 2); }
function VIR_QuickSlot3(%state) { if (%state) commandToServer('VIR_QuickSlot', 3); }
function VIR_QuickSlot4(%state) { if (%state) commandToServer('VIR_QuickSlot', 4); }

function VIR_AbilityBrickOverride(%index, %state) {
	if (!ServerConnection.isVIR || !$PlayingMiniGame) {
		return 0;
	}

	switch (%index) {
		case 1: VIR_Ability1(%state);
		case 2: VIR_Ability2(%state);
		case 3: VIR_Ability3(%state);
		case 4: VIR_Ability4(%state);
		case 5: VIR_Ability5(%state);
		case 6: VIR_Ability6(%state);
		case 7: VIR_QuickSlot1(%state);
		case 8: VIR_QuickSlot2(%state);
		case 9: VIR_QuickSlot3(%state);
		case 0: VIR_QuickSlot4(%state);
	}

	return 1;
}

if (!$VIR::AddedRemap) {
	$VIR::AddedRemap = 1;

	$remapDivision[$remapCount] = "Viridian RPG - General";
	appendRemap("Inventory (I)", "VIR_ToggleInventory", "keyboard\tctrl i");
	appendRemap("Character (U)", "VIR_ToggleInventory", "keyboard\tctrl u");
	// appendRemap("Ability 1 (Unbound)", "VIR_Ability1");
	// appendRemap("Ability 2 (Unbound)", "VIR_Ability2");
	// appendRemap("Ability 3 (Unbound)", "VIR_Ability3");
	// appendRemap("Ability 4 (Unbound)", "VIR_Ability4");
	// appendRemap("Ability 5 (Unbound)", "VIR_Ability5");
	// appendRemap("Ability 6 (Unbound)", "VIR_Ability6");
	// appendRemap("Quick Slot 1 (Unbound)", "VIR_QuickSlot1");
	// appendRemap("Quick Slot 2 (Unbound)", "VIR_QuickSlot2");
	// appendRemap("Quick Slot 3 (Unbound)", "VIR_QuickSlot3");
	// appendRemap("Quick Slot 4 (Unbound)", "VIR_QuickSlot4");

	$remapDivision[$remapCount] = "Viridian RPG - Magic";
	appendRemap("Self-Cast (MMB)", "VIR_SelfCast", "mouse0\tbutton2");
	// appendRemap("Area-Cast (Shift M2)", "VIR_AreaCast", "mouse0\tshift button1");
	appendRemap("Special-Cast (Ctrl Space)", "VIR_SpecialCast", "keyboard\tctrl space");
	appendRemap("Water (Alt Q)", "VIR_ConjureWater", "keyboard\talt q");
	appendRemap("Life (Alt W)", "VIR_ConjureLife", "keyboard\talt w");
	appendRemap("Shield (Alt E)", "VIR_ConjureShield", "keyboard\talt e");
	appendRemap("Cold (Alt R)", "VIR_ConjureCold", "keyboard\talt r");
	appendRemap("Lightning (Alt A)", "VIR_ConjureLightning", "keyboard\talt a");
	appendRemap("Death (Alt S)", "VIR_ConjureDeath", "keyboard\talt s");
	appendRemap("Earth (Alt D)", "VIR_ConjureEarth", "keyboard\talt d");
	appendRemap("Fire (Alt F)", "VIR_ConjureFire", "keyboard\talt f");

}

package VIRC_Binds
{
	function useBricks(%state)
	{
		if (!VIR_AbilityBrickOverride(1, %state))
			Parent::useBricks(%state);
	}

	function useFirstSlot(%state)
	{
		if (!VIR_AbilityBrickOverride(1, %state))
			Parent::useFirstSlot(%state);
	}

	function useSecondSlot(%state)
	{
		if (!VIR_AbilityBrickOverride(2, %state))
			Parent::useSecondSlot(%state);
	}

	function useThirdSlot(%state)
	{
		if (!VIR_AbilityBrickOverride(3, %state))
			Parent::useThirdSlot(%state);
	}

	function useFourthSlot(%state)
	{
		if (!VIR_AbilityBrickOverride(4, %state))
			Parent::useBricks(%state);
	}

	function useFifthSlot(%state)
	{
		if (!VIR_AbilityBrickOverride(5, %state))
			Parent::useFifthSlot(%state);
	}

	function useSixthSlot(%state)
	{
		if (!VIR_AbilityBrickOverride(6, %state))
			Parent::useSixthSlot(%state);
	}

	function useSeventhSlot(%state)
	{
		if (!VIR_AbilityBrickOverride(7, %state))
			Parent::useSeventhSlot(%state);
	}

	function useEighthSlot(%state)
	{
		if (!VIR_AbilityBrickOverride(8, %state))
			Parent::useEighthSlot(%state);
	}

	function useNinthSlot(%state)
	{
		if (!VIR_AbilityBrickOverride(9, %state))
			Parent::useNinthSlot(%state);
	}

	function useTenthSlot(%state)
	{
		if (!VIR_AbilityBrickOverride(0, %state))
			Parent::useTenthSlot(%state);
	}
};

activatePackage("VIRC_Binds");
