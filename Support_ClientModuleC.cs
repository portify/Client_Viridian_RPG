function registerClientModule(%id, %version)
{
  %entry = %id SPC %version;
  %count = getFieldCount($ClientModuleList);

  for (%i = 0; %i < %count; %i++)
  {
    %field = getField($ClientModuleList, %i);

    if (getWord(%field, 0) $= %id)
    {
      if (%version > getWord(%field, 1))
      {
        $ClientModuleList = setField($ClientModuleList, %i, %entry);
      }
      else
      {
        return;
      }
    }
  }

  $ClientModuleList = $ClientModuleList TAB %entry;
}

package ClientModuleCPackage
{
  function GameConnection::setConnectArgs(%this, %lan, %net, %pre, %suf, %rtb, %nonce, %modules, %a, %b, %c)
  {
    Parent::setConnectArgs(%this, %lan, %net, %pre, %suf, %rtb, %nonce, %modules @ $ClientModuleList, %a, %b, %c);
  }
};

activatePackage("ClientModuleCPackage");