﻿DG = {};
var t1;

$(document).ready(function () {
    DG.init();
    
});

DG = {
    init: function () {
        DG.util.animateSignup();
        DG.util.initLoginModal();
        DG.util.customizeDropdown();
        DG.util.customizeRadioBtn();
        DG.util.initRotateHero();
        DG.util.columns();
    }
};
DG.util = {
    customizeRadioBtn: function () {
        $(".radiox img").click(function (e) {
            var theValue = $(this).parent().find("input").prop('checked');
            $(".chkyes").hide();

            $(this).parent().find("input").prop('checked', true);
            $(this).parent().find(".chkyes").show();
            $(this).parent().find(".chkno").hide();

        });
        $(".radiox").next().click(function (e) {
            var theValue = $(this).prev().find("input").prop('checked');
            $(".chkyes").hide();

            $(this).prev().find("input").prop('checked', true);
            $(this).prev().find(".chkyes").show();
            $(this).prev().find(".chkno").hide();

        });
    },
    customizeDropdown: function () {
        var selectWidth, newWidth;
        $(".custom_dropdown").each(function (i) {
            selectWidth = $(this).find("select").width();
            newWidth = selectWidth - 30;
            $(this).width(newWidth);
        });
    },
    animateSignup: function(){
        $("#newSignup2 a").hover(function () {
            $("#newSignup2").css("background-position", "0 -457px");
        }, function () {
            $("#newSignup2").css("background-position", "0 0");
        });
    },
    initPin: function () {
        $(".pin a").hover(function () {
            clearTimeout(t1);
            $(".popup").hide();
            $(this).parent().next().show();
            $(".registration_map_content").css("z-index", 4);
            
        }, function () {
            DG.util.hidePopup();
            
        });

        $(".popup_content").hover(function () {
            clearTimeout(t1);
        }, function () {
            DG.util.hidePopup();
        });
    },
    hidePopup: function () {
        t1 = setTimeout(function () {
            $(".popup").hide();
            $(".registration_map_content").css("z-index", 2);
        }, 500);
    },
    initMapTab: function () {
        $(".nav-schedule").click(function (e) {
            e.preventDefault();
            $(".registration_list_singlecontent").hide();
            $(".reg-eventschedule").show();
            $("#registration_list_nav_content a.active").removeClass("active");
            $(this).addClass("active");
            //$("#mcs_container").mCustomScrollbar("vertical", 400, "easeOutCirc", 1.05, "auto", "yes", "yes", 10);
            $(".landingSchedule").mCustomScrollbar("destroy");
            $(".landingSchedule").mCustomScrollbar({
                set_width: 1040,
                set_height: 518,
                scrollButtons: {
                    scrollSpeed: 100,
                    scrollAmount: 60
                }
            });
        });

        $(".nav-info").click(function (e) {
            e.preventDefault();
            $(".registration_list_singlecontent").hide();
            $(".reg-eventinfo").show();
            $("#registration_list_nav_content a.active").removeClass("active");
            $(this).addClass("active");
            //$("#mcs_container1").mCustomScrollbar("vertical", 400, "easeOutCirc", 1.05, "auto", "yes", "yes", 10);
            $(".landingInfo").mCustomScrollbar("destroy");
            $(".landingInfo").mCustomScrollbar({
                set_width: 1040,
                set_height: 518,
                scrollButtons: {
                    scrollSpeed: 100,
                    scrollAmount: 60
                }
            });
        });
        $(".nav-faq").click(function (e) {
            e.preventDefault();
            $(".registration_list_singlecontent").hide();
            $(".reg-faq").show();
            $("#registration_list_nav_content a.active").removeClass("active");
            $(this).addClass("active");
            //$("#mcs_container2").mCustomScrollbar("vertical", 400, "easeOutCirc", 1.05, "auto", "yes", "yes", 10);
            $(".landingFaq").mCustomScrollbar("destroy");
            $(".landingFaq").mCustomScrollbar({
                set_width: 1040,
                set_height: 518,
                scrollButtons: {
                    scrollSpeed: 100,
                    scrollAmount: 60
                }
            });
        });
    },
    initEventTab: function () {
        $(".infotab-map").click(function (e) {
            e.preventDefault();
            $(".contentMain_info_singlecontent").hide();
            $(".contentMain_info_content .map").show();
            $(".contentMain_info_tab a.active").removeClass("active");
            $(this).addClass("active");
            google.maps.event.trigger(marker.map, 'resize');
            marker.map.setCenter(marker.position); // be sure to reset the map center as well

        });

        $(".infotab-eventinfo").click(function (e) {
            e.preventDefault();
            $(".contentMain_info_singlecontent").hide();
            $(".contentMain_info_content .info").show();
            $(".contentMain_info_tab a.active").removeClass("active");
            $(this).addClass("active");
            //$("#mcs_container1").mCustomScrollbar("vertical", 573, "easeOutCirc", 1.05, "auto", "yes", "yes", 10);
            $(".eventInfo").mCustomScrollbar("destroy");
            $(".eventInfo").mCustomScrollbar({
                set_width: 730,
                set_height: 534,
                scrollButtons: {
                    scrollSpeed: 100,
                    scrollAmount: 60
                }
            });
        });
        $(".infotab-faq").click(function (e) {
            e.preventDefault();
            $(".contentMain_info_singlecontent").hide();
            $(".contentMain_info_singlecontent.faq").show();
            $(".contentMain_info_tab a.active").removeClass("active");
            $(this).addClass("active");
            //$("#mcs_container2").mCustomScrollbar("vertical", 573, "easeOutCirc", 1.05, "auto", "yes", "yes", 10);
            $(".eventFaq").mCustomScrollbar("destroy");
            $(".eventFaq").mCustomScrollbar({
                set_width: 730,
                set_height: 534,
                scrollButtons: {
                    scrollSpeed: 100,
                    scrollAmount: 60
                }
            });
        });
    },
    initInviteFriendModal: function () {
        $(".showInviteFriend").click(function (e) {
            e.preventDefault();
            $("#overlay").show();
            $("#inviteFriendContainer").show();
            $('html, body').animate({ scrollTop: 300 }, 1500);
        });
        $(".closeInviteModal").click(function (e) {
            e.preventDefault();
            $("#overlay").hide();
            $("#inviteFriendContainer").hide();
        });
        $("#overlay").click(function (e) {
            e.preventDefault();
            $("#inviteFriendContainer").hide();
            $("#overlay").hide();
        });
    },
    initLoginModal: function () {
        $(".showloginmodal").live("click",function (e) {
            e.preventDefault();
            $("#login_modal").show();
            $("#overlay").show();
        });
        $("#overlay").click(function (e) {
            e.preventDefault();
            $("#login_modal").hide();
            $("#overlay").hide();
        });
        $("#closemodal").click(function (e) {
            e.preventDefault();
            $("#login_modal").hide();
            $("#overlay").hide();
        });
    },
    initRotateHero: function () {
        $(".hero img").hide();
        $(".hero img:first").show();
        var t1 = setTimeout(function () { DG.util.rotateHero(); }, 5000);
    },
    rotateHero: function () {
        var current, next;
        current = $(".hero img:visible");
        if ($(current).is(":last-child")) {
            next = $(".hero img:first");
        } else {
            next = $(current).next();
        }
        $(current).fadeOut(1000);
        $(next).fadeIn(1000);
        setTimeout(function () { DG.util.rotateHero(); }, 5000);
    },
    columns: function () {
        var theHeight = $(".contentMain").outerHeight();
        
        theHeight = theHeight - 80;
        $(".contentSidePanel_side").mCustomScrollbar("destroy");
        $(".contentSidePanel_side").mCustomScrollbar({
            set_width: 248,
            set_height: theHeight,
            scrollButtons: {
                scrollSpeed: 100,
                scrollAmount: 60
            }
        });
        //$(".contentSidePanel_side").height(theHeight);
        //var contentSideHeight = $(".contentSideFaq").outerHeight();

        //if (contentSideHeight > theHeight) {
        //    var faqHtml = $(".contentSideFaq").html();
        //    var scrollerFaq = '<div id="mcs_container1" class="sideFaqScroll"> \
	    //                    <div class="customScrollBox"> \
		//                    <div class="scroller_container"> \
		//                    <div class="scroller_content">'+ faqHtml + '</div> \
		//                    </div> \
		//                    <div class="dragger_container"> \
		//                    <div class="dragger"></div> \
		//                    </div> \
		//                    </div> \
        //                    </div>';

        //    $(".contentSideFaq").html(scrollerFaq);
        //    $(".sideFaqScroll").css("height", theHeight - 25);
        //    $(".sideFaqScroll .dragger_container").css("height", theHeight - 25);
        //    $("#mcs_container1").mCustomScrollbar("vertical", 248, "easeOutCirc", 1.05, "auto", "yes", "yes", 10);

        //}

    },
    registerDetailColumn: function () {
        var theHeight = $(".contentMain").outerHeight();

        theHeight = theHeight - 80;
        $(".contentSidePanel_side").mCustomScrollbar("destroy");
        $(".contentSidePanel_side").mCustomScrollbar({
            set_width: 268,
            set_height: theHeight,
            scrollButtons: {
                scrollSpeed: 100,
                scrollAmount: 60
            }
        });

        //$(".contentSidePanel_side").height(theHeight);
        //var contentSideHeight = $(".contentSideRegister").outerHeight();

        //if (contentSideHeight > theHeight) {
        //    var faqHtml = $(".contentSideRegister").html();
        //    var scrollerFaq = '<div id="mcs_container3" class="sideFaqScroll"> \
	    //                    <div class="customScrollBox"> \
		//                    <div class="scroller_container"> \
		//                    <div class="scroller_content">'+ faqHtml + '</div> \
		//                    </div> \
		//                    <div class="dragger_container"> \
		//                    <div class="dragger"></div> \
		//                    </div> \
		//                    </div> \
        //                    </div>';

        //    $(".contentSideRegister").html(scrollerFaq);
        //    $(".sideFaqScroll").css("height", theHeight);
        //    $(".sideFaqScroll .dragger_container").css("height", theHeight);
        //    $("#mcs_container3").mCustomScrollbar("vertical", 248, "easeOutCirc", 1.05, "auto", "yes", "yes", 10);

        //}
    },
    eventListScroller: function () {
        var theHeight = $("#EventDetail_List").outerHeight();

        theHeight = theHeight - 80;
        $(".landingSchedule").mCustomScrollbar("destroy");
        $(".landingSchedule").mCustomScrollbar({
            set_width: 1040,
            set_height: theHeight,
            scrollButtons: {
                scrollSpeed: 100,
                scrollAmount: 60
            }
        });
    },

    teamChatScrollbar: function () {
        var theHeight = $(".messageBoard").outerHeight();
        if (theHeight > 450) {
            $(".messageBoardContent").mCustomScrollbar("destroy");
            $(".messageBoardContent").mCustomScrollbar({
                set_width: 628,
                set_height: theHeight - 130,
                scrollButtons: {
                    scrollSpeed: 100,
                    scrollAmount: 60
                }
            });
            $(".messageBoardContent").mCustomScrollbar("scrollTo", "bottom");
            
        }
    },
    initEditRunBtn: function () {
        $(".button_editRun").click(function (e) {
            e.preventDefault();
            var links = $(this).parent().find(".editReg").html();
            $("#overlay").show();
            $("#editRunContainer").show();
            $("#editRunContent").html(links);
            $('html, body').animate({ scrollTop: 300 }, 1500);

            $(".closeEditRunModal").click(function (e) {
                e.preventDefault();
                $("#overlay").hide();
                $("#editRunContainer").hide();
                $("#editRunContent").html("");
            });
            $("#overlay").click(function (e) {
                e.preventDefault();
                $("#editRunContainer").hide();
                $("#overlay").hide();
                $("#editRunContent").html("");
            });
        });
    },
    openRedeemCode: function () {
        $(".openRedeemCode").click(function (e) {
            e.preventDefault();
            $(".redemptionCodesContent").toggle(600);
        });
    },
    getFaqUrl: function () {
        var url = window.location.href;
        if (url.toLowerCase().indexOf("?landingfaq") != -1) {
            window.location.href = url + "#EventList";
            $(".nav-faq").click();
        }
    },
    getInfoUrl: function () {
        var url = window.location.href;
        if (url.toLowerCase().indexOf("?landinginfo") != -1) {
            window.location.href = url + "#EventList";
            $(".nav-info").click();
        }
    }
};