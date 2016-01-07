function clientCmdZoom(%zoom)
{
  if (%zoom $= "")
  {
    %zoom = 1;
  }

  %zoom = mClampF(%zoom, 0, 2);

  if (isObject(ServerConnection))
  {
    ServerConnection.zoom = %zoom;

    if (!isEventPending(ServerConnection.updateZoom))
    {
      ServerConnection.updateZoom(1);
    }
  }
}

function clientCmdAimRecoil(%x, %y, %ticks)
{
  if (%ticks > 100)
  {
    return;
  }

  aimRecoil(mDegToRad(%x / %ticks), mDegToRad(%y / %ticks), %ticks, 0);
}

function aimRecoil(%x, %y, %ticks, %i, %flipped)
{
  if (%i < 0 || %i >= %ticks && %flipped)
  {
    return;
  }

  if (%i >= %ticks)
  {
    %flipped = 1;
    %i = 0;

    %x *= -1;
    %y *= -1;
  }

  $mvYaw += %x;
  $mvPitch += %y;

  schedule(%flipped ? 16 : 0, 0, "aimRecoil", %x, %y, %ticks, %i + 1, %flipped);
}

function GameConnection::updateZoom(%this, %zoom)
{
  cancel(%this.updateZoom);

  %zoom = mClampF(%zoom, 0, 2);
  %zoom = %this.zoom;

  PlayGUI.forceFOV = %zoom == 1 ? 0 : $cameraFov * %zoom;

  if (%zoom == %this.zoom)
  {
    return;
  }

  %speed = 0.05;
  %delta = mClampF(%this.zoom - %zoom, -%speed, %speed);

  %this.updateZoom = %this.schedule(16, "updateZoom", %zoom + %delta);
}

package WeaponZoomPackage
{
  function getMouseAdjustAmount(%value)
  {
    %parent = Parent::getMouseAdjustAmount(%value);

    if (ServerConnection.zoom !$= "" && ServerConnection.zoom != 1)
    {
      // Let's not screw them over as much
      %modified = %value * (ServerConnection.zoom * $cameraFov / 90) * 0.001;
      // return (%modified + %parent) / 2;
      // Nevermind that's even worse
      return %modified;
    }

    return %parent;
  }
};

activatePackage("WeaponZoomPackage");