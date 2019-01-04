TweenParms= Holoville.HOTween.TweenParms;
EaseType = Holoville.HOTween.EaseType;
HOTween = Holoville.HOTween.HOTween;

TweenParmsExtendedCSWrap = SoftStar.GWY.Tools.TweenParmsExtendedCSWrap;
WYTween = SoftStar.GWY.Tools.WYTween;



UIPage ={
	none = "None" ,
	GameHall = "GameHall",
	N11 = "n11",
	N12 = "n12",
}

function UIPage.PopoutAnim(transfrom)
	HOTween.Kill(transfrom);
	local tween=TweenParms.New();
	tween:Prop("localScale",Vector3.New(0,0,0));
	tween:Ease(EaseType.EaseOutBounce);
	HOTween.From(transfrom,0.3,tween);
	HOTween.Restart(tween);
end

function UIPage.AlphaPanel(transfrom)
	local panel = GetComponent(transfrom,"UIPanel");
	HOTween.Kill(panel);
	local tween=TweenParms.New();
	tween:Prop("alpha",0);
	tween:Ease(EaseType.EaseInBack);
	HOTween.From(panel,0.5,tween);
	HOTween.Restart(tween);
end