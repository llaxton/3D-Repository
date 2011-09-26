﻿Type.registerNamespace("AjaxControlToolkit");AjaxControlToolkit.RatingBehavior=function(c){var b=null,a=this;AjaxControlToolkit.RatingBehavior.initializeBase(a,[c]);a._starCssClass=b;a._filledStarCssClass=b;a._emptyStarCssClass=b;a._waitingStarCssClass=b;a._readOnly=false;a._ratingValue=0;a._currentRating=0;a._maxRatingValue=5;a._tag="";a._ratingDirection=0;a._stars=b;a._callbackID=b;a._mouseOutHandler=Function.createDelegate(a,a._onMouseOut);a._starClickHandler=Function.createDelegate(a,a._onStarClick);a._starMouseOverHandler=Function.createDelegate(a,a._onStarMouseOver);a._keyDownHandler=Function.createDelegate(a,a._onKeyDownBack);a._autoPostBack=false};AjaxControlToolkit.RatingBehavior.prototype={initialize:function(){var a=this;AjaxControlToolkit.RatingBehavior.callBaseMethod(a,"initialize");var c=a.get_element();a._stars=[];for(var b=1;b<=a._maxRatingValue;b++){starElement=$get(c.id+"_Star_"+b);starElement.value=b;Array.add(a._stars,starElement);$addHandler(starElement,"click",a._starClickHandler);$addHandler(starElement,"mouseover",a._starMouseOverHandler)}$addHandler(c,"mouseout",a._mouseOutHandler);$addHandler(c,"keydown",a._keyDownHandler);a._update()},dispose:function(){var a=this,d=a.get_element();if(a._stars){for(var b=0;b<a._stars.length;b++){var c=a._stars[b];$removeHandler(c,"click",a._starClickHandler);$removeHandler(c,"mouseover",a._starMouseOverHandler)}a._stars=null}$removeHandler(d,"mouseout",a._mouseOutHandler);$removeHandler(d,"keydown",a._keyDownHandler);AjaxControlToolkit.RatingBehavior.callBaseMethod(a,"dispose")},_onError:function(a){alert(String.format(AjaxControlToolkit.Resources.Rating_CallbackError,a))},_receiveServerData:function(b,a){a._waitingMode(false);a.raiseEndClientCallback(b)},_onMouseOut:function(){var a=this;if(a._readOnly)return;a._currentRating=a._ratingValue;a._update();a.raiseMouseOut(a._currentRating)},_onStarClick:function(){var a=this;if(a._readOnly)return;a._ratingValue!=a._currentRating&&a.set_Rating(a._currentRating)},_onStarMouseOver:function(b){var a=this;if(a._readOnly)return;if(a._ratingDirection==0)a._currentRating=b.target.value;else a._currentRating=a._maxRatingValue+1-b.target.value;a._update();a.raiseMouseOver(a._currentRating)},_onKeyDownBack:function(b){var a=this;if(a._readOnly)return;var c=b.keyCode?b.keyCode:b.rawEvent.keyCode;if(c==Sys.UI.Key.right||c==Sys.UI.Key.up){a._currentRating=Math.min(a._currentRating+1,a._maxRatingValue);a.set_Rating(a._currentRating);b.preventDefault();b.stopPropagation()}else if(c==Sys.UI.Key.left||c==Sys.UI.Key.down){a._currentRating=Math.max(a._currentRating-1,1);a.set_Rating(a._currentRating);b.preventDefault();b.stopPropagation()}},_waitingMode:function(d){var a=this;for(var c=0;c<a._maxRatingValue;c++){var b;if(a._ratingDirection==0)b=a._stars[c];else b=a._stars[a._maxRatingValue-c-1];if(a._currentRating>c)if(d){Sys.UI.DomElement.removeCssClass(b,a._filledStarCssClass);Sys.UI.DomElement.addCssClass(b,a._waitingStarCssClass)}else{Sys.UI.DomElement.removeCssClass(b,a._waitingStarCssClass);Sys.UI.DomElement.addCssClass(b,a._filledStarCssClass)}else{Sys.UI.DomElement.removeCssClass(b,a._waitingStarCssClass);Sys.UI.DomElement.removeCssClass(b,a._filledStarCssClass);Sys.UI.DomElement.addCssClass(b,a._emptyStarCssClass)}}},_update:function(){var a=this,d=a.get_element();$get(d.id+"_A").title=a._currentRating;for(var c=0;c<a._maxRatingValue;c++){var b;if(a._ratingDirection==0)b=a._stars[c];else b=a._stars[a._maxRatingValue-c-1];if(a._currentRating>c){Sys.UI.DomElement.removeCssClass(b,a._emptyStarCssClass);Sys.UI.DomElement.addCssClass(b,a._filledStarCssClass)}else{Sys.UI.DomElement.removeCssClass(b,a._filledStarCssClass);Sys.UI.DomElement.addCssClass(b,a._emptyStarCssClass)}}},add_Rated:function(a){this.get_events().addHandler("Rated",a)},remove_Rated:function(a){this.get_events().removeHandler("Rated",a)},raiseRated:function(b){var a=this.get_events().getHandler("Rated");a&&a(this,new AjaxControlToolkit.RatingEventArgs(b))},add_MouseOver:function(a){this.get_events().addHandler("MouseOver",a)},remove_MouseOver:function(a){this.get_events().removeHandler("MouseOver",a)},raiseMouseOver:function(b){var a=this.get_events().getHandler("MouseOver");a&&a(this,new AjaxControlToolkit.RatingEventArgs(b))},add_MouseOut:function(a){this.get_events().addHandler("MouseOut",a)},remove_MouseOut:function(a){this.get_events().removeHandler("MouseOut",a)},raiseMouseOut:function(b){var a=this.get_events().getHandler("MouseOut");a&&a(this,new AjaxControlToolkit.RatingEventArgs(b))},add_EndClientCallback:function(a){this.get_events().addHandler("EndClientCallback",a)},remove_EndClientCallback:function(a){this.get_events().removeHandler("EndClientCallback",a)},raiseEndClientCallback:function(b){var a=this.get_events().getHandler("EndClientCallback");a&&a(this,new AjaxControlToolkit.RatingCallbackResultEventArgs(b))},get_AutoPostBack:function(){return this._autoPostBack},set_AutoPostBack:function(a){this._autoPostBack=a},get_Stars:function(){return this._stars},get_Tag:function(){return this._tag},set_Tag:function(a){if(this._tag!=a){this._tag=a;this.raisePropertyChanged("Tag")}},get_CallbackID:function(){return this._callbackID},set_CallbackID:function(a){this._callbackID=a},get_RatingDirection:function(){return this._ratingDirection},set_RatingDirection:function(b){var a=this;if(a._ratingDirection!=b){a._ratingDirection=b;a.get_isInitialized()&&a._update();a.raisePropertyChanged("RatingDirection")}},get_EmptyStarCssClass:function(){return this._emptyStarCssClass},set_EmptyStarCssClass:function(a){if(this._emptyStarCssClass!=a){this._emptyStarCssClass=a;this.raisePropertyChanged("EmptyStarCssClass")}},get_FilledStarCssClass:function(){return this._filledStarCssClass},set_FilledStarCssClass:function(a){if(this._filledStarCssClass!=a){this._filledStarCssClass=a;this.raisePropertyChanged("FilledStarCssClass")}},get_WaitingStarCssClass:function(){return this._waitingStarCssClass},set_WaitingStarCssClass:function(a){if(this._waitingStarCssClass!=a){this._waitingStarCssClass=a;this.raisePropertyChanged("WaitingStarCssClass")}},get_Rating:function(){var a=this;a._ratingValue=AjaxControlToolkit.RatingBehavior.callBaseMethod(a,"get_ClientState");if(a._ratingValue=="")a._ratingValue=null;return a._ratingValue},set_Rating:function(b){var a=this;if(a._ratingValue!=b){a._ratingValue=b;a._currentRating=b;if(a.get_isInitialized()){if(b<0||b>a._maxRatingValue)return;a._update();AjaxControlToolkit.RatingBehavior.callBaseMethod(a,"set_ClientState",[a._ratingValue]);a.raisePropertyChanged("Rating");a.raiseRated(a._currentRating);a._waitingMode(true);var c=a._currentRating+";"+a._tag,d=a._callbackID;if(a._autoPostBack)__doPostBack(d,c);else WebForm_DoCallback(d,c,a._receiveServerData,a,a._onError,true)}}},get_MaxRating:function(){return this._maxRatingValue},set_MaxRating:function(a){if(this._maxRatingValue!=a){this._maxRatingValue=a;this.raisePropertyChanged("MaxRating")}},get_ReadOnly:function(){return this._readOnly},set_ReadOnly:function(a){if(this._readOnly!=a){this._readOnly=a;this.raisePropertyChanged("ReadOnly")}},get_StarCssClass:function(){return this._starCssClass},set_StarCssClass:function(a){if(this._starCssClass!=a){this._starCssClass=a;this.raisePropertyChanged("StarCssClass")}}};AjaxControlToolkit.RatingBehavior.registerClass("AjaxControlToolkit.RatingBehavior",AjaxControlToolkit.BehaviorBase);AjaxControlToolkit.RatingEventArgs=function(a){AjaxControlToolkit.RatingEventArgs.initializeBase(this);this._rating=a};AjaxControlToolkit.RatingEventArgs.prototype={get_Rating:function(){return this._rating}};AjaxControlToolkit.RatingEventArgs.registerClass("AjaxControlToolkit.RatingEventArgs",Sys.EventArgs);AjaxControlToolkit.RatingCallbackResultEventArgs=function(a){AjaxControlToolkit.RatingCallbackResultEventArgs.initializeBase(this);this._result=a};AjaxControlToolkit.RatingCallbackResultEventArgs.prototype={get_CallbackResult:function(){return this._result}};AjaxControlToolkit.RatingCallbackResultEventArgs.registerClass("AjaxControlToolkit.RatingCallbackResultEventArgs",Sys.EventArgs);