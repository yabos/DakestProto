﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : SceneBase
{
    public override IEnumerator OnEnter(float progress)
    {
        yield return base.OnEnter(progress);

        yield return Global.UIMgr.OnCreateWidgetAsync<UILobby>(ResourcePath.UILobby, widget =>
        {
            if (widget != null)
            {
                Global.SoundMgr.PlayBGM(SoundManager.eBGMType.eBGM_Lobby);

                widget.OwnerScene = this;
                widget.Show();
                SetEnterPageProgressInfo(0.5f);
            }
        });

        yield return new WaitForSeconds(1.0f);
    }

    public override void OnExit()
    {
        base.OnExit();

        Global.UIMgr.HideAllWidgets(0.3f);
    }

    public override void OnInitialize()
    {

    }

    public override void OnFinalize()
    {
    }

    public override void OnRequestEvent(string netClentTypeName, string requestPackets)
    {
        Global.UIMgr.ShowLoadingWidget(0.3f);
    }

    public override void OnReceivedEvent(string netClentTypeName, string receivePackets)
    {
        Global.UIMgr.HideLoadingWidget(0.1f);
    }

    //private void ShowMessageBoxWithPluginNotifyInfo(string message, eMessageBoxType boxType = eMessageBoxType.OK, System.Action<bool> completed = null)
    //{
    //    string title = StringUtil.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(Color.blue), "System Info Message");
    //    Global.WidgetMgr.ShowMessageBox(title, message, boxType, completed);
    //}

    public override void OnNotify(INotify notify)
    {
        base.OnNotify(notify);
    }
}