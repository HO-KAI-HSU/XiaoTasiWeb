// JavaScript Document 
$(function () {
	const $header_right = $(".header_right");
	const $header_box = $(".header_box");
	const $policy_btn = $(".policy_btn");
	const $rule_btn = $(".rule_btn");
	const $mask = $(".mask");
	const $login_modal = $(".login_modal");
	const $register_member_modal = $(".register_member_modal");
	const $phone_verification_modal = $(".phone_verification_modal");
	const $forget_psw_modal = $(".forget_psw_modal");
	const $reset_psw_modal = $(".reset_psw_modal");
	const $add_member_modal = $(".add_member_modal");
	const $video_modal = $(".video_modal");
	const $upload_pay_modal = $(".upload_pay_modal");
	const $fixed_btn_box = $(".fixed_btn_box");
	const $mobile_menu = $("#mobile_menu");
	const $trip_more_btn = $(".trip_more_btn");
	const $go_top_btn = $(".go_top_btn");
	refreshSubMenu();

	//Mobile Menu
	function open() {
		$header_right.slideToggle('4600', "easeInOutBack");
		$mask.fadeToggle(500);
	}
	function clear_style() {
		if ($(window).innerWidth() > 1024) {
			$(".header_right").attr("style", "");
		}
	}
	$mobile_menu.on("click", function () {
		$(this).toggleClass("open");
		open();
	});
	$(window).on("resize", function () {
		clear_style();
	});

	//Modal
	function menu_toggle() {
		$mobile_menu.removeClass("open");
		$mask.show();
	}
	if ($(window).innerWidth() > 1250) {
		$(".login_in_btn,.member_login_btn,.forget_psw,.register_btn,.agree_btn,.add_member,.disagree_btn,photo_video,img.multiple_day_video_photo,.video_close_model,.upload_pay_btn").on("click", function () {
			menu_toggle();
		});
	}
	if ($(window).innerWidth() <= 1250) {
		$(".login_in_btn,.member_login_btn,.forget_psw,.register_btn,.agree_btn,.add_member,.disagree_btn,photo_video,img.multiple_day_video_photo,.video_close_model,.upload_pay_btn").on("click", function () {
			$header_right.slideUp('4600', "easeInOutBack");
			menu_toggle();
		});
	} 
	$(".login_in_btn").on("click", function () {
		$mask.show();
		$login_modal.show();	
	});
	$(".member_login_btn").on("click", function () {
		$(".add_member_modal,.register_member_modal").hide();
		$(".member_number_modal").show();
	});
	$(".forget_psw").on("click", function () {
		$login_modal.hide();
		$forget_psw_modal.show();
	});
	$(".modal_close_icon").on("click", function () {
		$(".login_modal,.forget_psw_modal,.register_member_modal,.add_member_modal,.phone_verification_modal,.upload_pay_modal,.reset_psw_modal").hide();
		$mask.hide();
	});
	$("a.register_btn").on("click", function () {
		$mask.show();
		$register_member_modal.show();
	});
	$("a.add_member").on("click", function () {
		$register_member_modal.show();
	});
	$("a.agree_btn").on("click", function () {
		$register_member_modal.hide();
		$add_member_modal.show();
	});
	$(".disagree_btn").on("click", function () {
		$(".login_modal,.register_member_modal").hide();
		$mask.hide();
	})
	$(".alert_close_btn").on("click", function () {
		$(".alert.success, .alert.warning").css("display", "none"); 
	});
	$(".photo_video,img.multiple_day_video_photo").on("click", function () {
		console.log("multiple_day_video_photo");
		$mask.show();
		$video_modal.show();
	});
	$(document).on("click", "img.multiple_day_video_photo", function () {
		console.log("multiple_day_video_photo");
		$mask.show();
		$video_modal.show();
	});
	$(".video_close_model").on("click", function () {
		$video_modal.hide();
		$mask.hide();
	});
	$(document).on("click", ".upload_pay_btn, .reupload_pay_btn", function () {
		var travelReservationCode = $(this).closest("tr").find("input#travelReservationCode").val();
		localStorage.setItem("travelReservationCode", travelReservationCode);
		console.log("upload_pay_btn");
		$mask.show();
		$upload_pay_modal.show();
	});
	$(document).on("click", ".send_reservation_check_btn", function () {
		var loginInfo = JSON.parse(localStorage.getItem('loginInfo'));
		var picPath = localStorage.getItem("picPath");
		var travelReservationCode = localStorage.getItem("travelReservationCode");
		localStorage.removeItem("picPath");
		localStorage.removeItem("travelReservationCode");
		var _token = loginInfo.token;
		console.log("send_reservation_check_btn");
		let dataJson = {};
		var bankAccountCode = $(this).closest("div").find("input#bank_number").val();
		console.log(bankAccountCode);
		dataJson['token'] = _token;
		dataJson['travelReservationCode'] = travelReservationCode;
		dataJson['bankAccountCode'] = bankAccountCode;
		dataJson['travelReservationCheckPicPath'] = picPath;
		addReservationCheck(dataJson);
		$upload_pay_modal.hide();
		$mask.hide();
	});

	$(document).on("click", ".cancel_btn", function () {
		var travelReservationCode = $(this).closest("tr").find("input#travelReservationCode").val();
		var _token = JSON.parse(localStorage.getItem('loginInfo')).token;
		cancelReservation(_token, travelReservationCode);
	});

	$(document).on("click", ".choose_photo_file_btn", function () {
		console.log("choose_photo_file_btn");
		var file = $("#upload_pay_photo").get(0).files;

	});
	$("#upload_pay_photo").change(function () {
		var file = $("#upload_pay_photo").get(0).files;
		uploadPic(file, 1);
	});


	$("ul.travel_tabs").find("li").on("click",function(){
		let $travel_tab = $(this).find("a").attr("href");
		let $travel_tab_title = $(this).attr("id");
		$(".tab_content[class!='tab_content_1']").hide();
		$(this).addClass("travel_btn_current").siblings().removeClass("travel_btn_current");
		$($travel_tab).fadeIn(300);
		switch ($travel_tab_title) {
			case "multi_day_trip":
				multipledayTripList(2, "tab_1");
				$("#tab_1").show();
				break;
			case "island_trip":
				multipledayTripList(3, "tab_2");
				$("#tab_2").show();
				break;
			case "car_trip":
				multipledayTripList(4, "tab_3");
				$("#tab_3").show();
				break;
			case "foreign_trip":
				multipledayTripList(5, "tab_4");
				$("#tab_4").show();
				break;
		}
		return false;
	});

	//Tab   
	$(".travel_schedule_btn").on("click", function () {
		let $travel_theme = $(this).attr("id");
		switch ($travel_theme) {
			case "multi_day_trip":
				$trip_more_btn.attr("href", "/Trip/MultipledayTrip");
				break;
			case "island_trip":
				$trip_more_btn.attr("href", "/Trip/IslandTrip");
				break;
			case "car_trip":
				$trip_more_btn.attr("href", "/Trip/CarTrip");
				break;
			case "foreign_trip":
				$trip_more_btn.attr("href", "/Trip/ForeignTrip");
				break;
			case "single_day_trip":
				window.location.href = "/Trip/SingledayTrip";
				break;
		}
	});

	$(".two_day_tabs > li:not(.two_day_tabs li:last()),.multiple_day_travel_list_tabs > li,.tour_bus_choose_box > li").on("click", function () {
		let $now_tab = $(this).find("a").attr("href");
		$(".tab_content[class!='tab_content_1']").hide();
		$(this).addClass("tab_current").siblings().removeClass("tab_current");
		$($now_tab).fadeIn(300);
		return false;
	});
	$(".month_choose > li").on("click", function () {
		let $month_tab = $(this).find("a").attr("href");
		$(".month_content[class!='month_content_1']").hide();
		$(this).addClass("tab_current").siblings().removeClass("tab_current");
		$($month_tab).fadeIn(300);
		return false;
	});
	$(".two_day_tabs li:last").on("click", function () {
		$(".two_day_tabs").find("li").removeClass("tab_current");
		$(this).addClass("tab_current");
		window.location.replace("https://docs.google.com/forms/d/1-usgHugOctshpg8lj9xrSMe9mCax97DGKXF-Yh1Dmsg/edit?hl=zh-tw");
	});
	$(".rule_tabs > li").on("click", function () {
		let $now_tab = $(this).find("a").attr("href");
		$(".tab_content[class!='tab_content_1']").hide();
		$(this).addClass("rule_current").siblings().removeClass("rule_current");
		$($now_tab).fadeIn(300);
		if ($policy_btn.hasClass("rule_current")) {
			$policy_btn.find("img").attr("src", "/images/arrow_icon_orange" + ".png");
		} else {
			$policy_btn.find("img").attr("src", "/images/arrow_icon_blue" + ".png");
		}
		if ($rule_btn.hasClass("rule_current")) {
			$rule_btn.find("img").attr("src", "/images/arrow_icon_orange" + ".png");
		} else {
			$rule_btn.find("img").attr("src", "/images/arrow_icon_blue" + ".png");
		}
		return false;
	});


	//Travel
	$(".tabs_brief_btn").on("click", function () {
		$(this).toggleClass("show");
		if ($(".timeline_travel").is(":visible")) {
			$(this).text("精簡模式");
		} else {
			$(this).text("圖文模式");
		}
		$(".timeline_travel,.travel_table").toggle();
	});

	//Slider 
	$(".index_slider,.video_slider").slick({
		arrows: false,
		slidesToShow: 1,
		infinite: true,
		dots: true,
		autoplay: true
	});
	$(".news_slider").slick({
		dots: false,
		infinite: true,
		speed: 300,
		slidesToShow: 4,
		slidesToScroll: 4,
		responsive: [
			{
				breakpoint: 1250,
				settings: {
					slidesToShow: 1,
					slidesToScroll: 1,
					infinite: true,
					dots: false
				}
			},
			{
				breakpoint: 480,
				settings: {
					slidesToShow: 1,
					slidesToScroll: 1
				}
			}
		]
	});


	//Calendar
	$("ul.month_choose").find("li").on("click", function () {
		console.log("month_choose");
		$(this).addClass("selected").siblings().removeClass("selected");
	});

	$(".month_travel_list").on("click", function () {
		console.log("month_travel_list");
		$(this).addClass("selected").siblings().removeClass("selected");
	});

	//Go_Top
	$("#go_top_btn").on("click",function(){ 
		$("html,body").stop().animate({
			scrollTop: $header_box
		}, 800);
		return false;
	});

	$(".login_btn").on("click", function () {
		login(closeLoginModel);
	});

	$(".logout_btn").on("click", function () {
		logout();
	});

	$(".add_member_btn").on("click", function () {
		registerCheck($add_member_modal, $phone_verification_modal);
	});

	$(".submit_verification_btn").on("click", function () {
		var verificationType = $("#verification_type").val();
		if (verificationType == "1") {
             // 註冊使用
			var cellphone = $("#phone").val();
			verify(cellphone, register("", "", closePhoneVerificationModel));
		} else if (verificationType == "2") {
             // 忘記密碼使用 
			var cellphone = $("#forget_phone").val();
			console.log(cellphone);
			verify(cellphone, showResetPwdModel(closePhoneVerificationModel));
        }
	});

	$(".resend_verification_btn").on("click", function () {
		var cellphone = $("#phone").val();
		var verificationType = $("#verification_type").val();
		if (verificationType == "2") {
			// 忘記密碼使用   
			var cellphone = $("#forget_phone").val();
		}
		sendVerificationNumber(verificationType, cellphone);
		settime($("a.resend_verification_btn"), 60);
	});

	$(".forget_psw_btn").on("click", function () {
		$("#verification_type").val("2");
		var cellphone = $("#forget_phone").val();
		sendVerificationNumber("2", cellphone, $forget_psw_modal, $phone_verification_modal);
		settime($("a.resend_verification_btn"), 60);
	});

	$(".reset_psw_btn").on("click", function () {
		resetPwd(closeResetPwdModel);
	});

	//Go Top
	$(window).on("scroll", function () {
		if ($(this).scrollTop() > $(".banner_box_height").offset().top) {
			$fixed_btn_box.fadeIn(500);
		} else {
			$fixed_btn_box.fadeOut(500);
		}
	});
	if ($(window).innerWidth() <= 1250) {
		$(window).on("scroll", function () {
			if ($(this).scrollTop() >= $(".mobile_footer_box").offset().top / 1.15) {
				$go_top_btn.addClass("lightblue");
			} else {
				$go_top_btn.removeClass("lightblue");
			}
		});
	}
	$(".desktop_go_top_btn,.go_top_btn").on("click", function () {
		$("html,body").stop().animate({ scrollTop: $(".header_box").offset().top }, 800, "easeOutCubic");
		return false;
	});

	function closePhoneVerificationModel() {
		$mask.hide();
		$phone_verification_modal.hide();
		settime($("a.resend_verification_btn"), 0);
	}

	function showResetPwdModel(closePhoneVerificationModel) {
		closePhoneVerificationModel();
		$mask.show();
		$reset_psw_modal.show();
	}
	function closeResetPwdModel() {
		console.log("closeResetPwdModel");
		$mask.hide();
		$reset_psw_modal.hide();
	}
	function closeLoginModel() {
		$mask.hide();
		$login_modal.hide();
	}
});

function multipledayTripList(travelType = 0, tabName = null) {
	$.post('/Trip/GetTravelListForMember', { page: 1, limit: 100, travelType: travelType }).done(function (tripList) {
		var item = "";
		$.each(tripList.travelList, function (i, trip) {
			if (i == 0 && i % 3 == 0) {
				// 第一次
				var row = 0;
				item = `<ul class="news_content clearfix mt0">`;
			} else if (i != 0 && i % 3 == 0) {
				// 第N次
				row++;
				item += `</ul>`;
				item += `<ul class="news_content clearfix">`;
			}

			item += `<li>
                    <a href="#" id="tripInfo"><img src="/images/multipleday_trip_photo2.png" alt="台北近郊一日遊" title="台北近郊一日遊"></a>
                    <input type="hidden" id="travelCode" value="${trip.travelCode}">
                    <span class="date">${trip.travelFdate}</span>
                    <h4>${trip.travelTraditionalTitle}</h4>
                    <p>出發日期:<span>${trip.startDate}</span></p>
                    <p class="travel_fee">旅費:<span class="dollars">${trip.cost}</span>元起</p>
                    </li>`;
			if (i == (tripList.length - 1)) {
				item += `</ul>`;
			}
		});
		$('#' + tabName).html(item);
	});
}


function tripInfo(travelCode = null, _travelId = null, indexFlag = false) {
	var travelCode = travelCode;
	switch (_travelId) {
		case "1":
	        window.location.href = indexFlag ? "Trip/MultipledayTripInfo?travelCode=" + travelCode : 'MultipledayTripInfo?travelCode=' + travelCode;
			break;
		case "2":
	        window.location.href = indexFlag ? "Trip/IslandTripInfo?travelCode=" + travelCode : 'IslandTripInfo?travelCode=' + travelCode;
			break;
		case "3":
	        window.location.href = indexFlag ? "Trip/CarTripInfo?travelCode=" + travelCode : 'CarTripInfo?travelCode=' + travelCode;
			break;
		case "4":
	        window.location.href = indexFlag ? "Trip/ForeignTripInfo?travelCode=" + travelCode : 'ForeignTripInfo?travelCode=' + travelCode;
			break;
	}
}


function login(closeLoginModel) {
	const now = new Date();
	var number = $("#number").val();
	var psw = $("#psw").val();
	$.post('/Login/login', { username: number, password: psw }).done(function (loginRes) {
		var reason = loginRes.reason;
		if (loginRes.success === 1) {
			closeLoginModel();
			var exp = parseInt(now.getTime() / 1000) + 7200;
			console.log(exp);
			console.log(loginRes.data);
			loginRes.data.expired = exp;
			localStorage.setItem("loginInfo", JSON.stringify(loginRes.data));
			showAlert(true, "登入成功", function () { window.location.href = ""; });
		} else {
			showAlert(false, reason);
		}
	});
}

// 註冊參數判斷 
function registerCheck(add_member_modal, phone_verification_modal) {
	var number = $("#username").val();
	var password = $("#password").val();
	var name = $("#name").val();
	var email = $("#email").val();
	var address = "";;
	var birthday = $("#date").val();
	var telephone = "";
	var cellphone = $("#phone").val();
	var emerContactName = $("#contact_name").val();
	var emerContactPhone = $("#contact_phone").val();
	var success = 0;
	var errorCount = 0;
	if (number == null || number == "") {
		errorCount++;
		$("#username").css('border', "2px solid rgba(255, 99, 71, 0.8)");
	} else {
		$("#username").css('border', "");
    }
	if (password == null || password == "") {
		errorCount++;
		$("#password").css('border', "2px solid rgba(255, 99, 71, 0.8)");
	} else {
		$("#password").css('border', "");
    }
	if (birthday == null || birthday == "") {
		errorCount++;
		$("#date").css('border', "2px solid rgba(255, 99, 71, 0.8)");
	} else {
		$("#date").css('border', "");
    }
	if (name == null || name == "") {
		errorCount++;
		$("#name").css('border', "2px solid rgba(255, 99, 71, 0.8)");
	} else {
		$("#name").css('border', "");
    }
	if (cellphone == null || cellphone == "") {
		errorCount++;
		$("#phone").css('border', "2px solid rgba(255, 99, 71, 0.8)");
	} else {
		$("#phone").css('border', "");
    }
	console.log(errorCount);
	if (errorCount > 0) {
		return true;
	}

	$.post('/Register/registerByPhone', { username: number, password: password, name: name, email: email, address: address, birthday: birthday, telephone: telephone, cellphone: cellphone, emerContactName: emerContactName, emerContactPhone: emerContactPhone, checkFlag: 1 }).done(function (registerRes) {
		var reason = registerRes.reason;
		success = registerRes.success;
		if (success === 1) {
			$("#verification_type").val("1");
			var cellphone = $("#phone").val();
			sendVerificationNumber("1", cellphone, add_member_modal, phone_verification_modal);
		} else {
			showAlert(false, reason);
		}
	});
	return true;
}

// 註冊帳戶  
function register(add_member_modal, phone_verification_modal, closePhoneVerificationModel) {
	var number = $("#username").val();
	var password = $("#password").val();
	var name = $("#name").val();
	var email = $("#email").val();
	var address = "";;
	var birthday = $("#date").val();
	var telephone = "";
	var cellphone = $("#phone").val();
	var emerContactName = $("#contact_name").val();
	var emerContactPhone = $("#contact_phone").val();
	var success = 0;
	$.post('/Register/registerByPhone', { username: number, password: password, name: name, email: email, address: address, birthday: birthday, telephone: telephone, cellphone: cellphone, emerContactName: emerContactName, emerContactPhone: emerContactPhone, checkFlag: 0 }).done(function (registerRes) {
		var reason = registerRes.reason;
		success = registerRes.success;
		if (success === 1) {
			showAlert(true, reason);
			closePhoneVerificationModel();
		} else {
			showAlert(false, reason);
        }
	});
	return true;
}

// 發送驗證碼
function sendVerificationNumber(verificationType = "", cellphone = "", modal1 = "", modal2 = "") {
	var success = 0;
	console.log(verificationType);
	console.log(cellphone);
	var timelocalStor = localStorage.getItem("timeId");
	if (timelocalStor == null) {
		$.post('/Register/getPhoneCaptcha', { cellphone: cellphone, verificationType: verificationType }).done(function (sendVerificationNumberRes) {
			var reason = sendVerificationNumberRes.reason;
			success = sendVerificationNumberRes.success;
			if (success === 1) {
				showAlert(true, reason);
				if (modal1 != "" && modal2 != "") {
					modal1.hide();
					modal2.show();
					settime($(".resend_verification_btn"), 60);
				}
			} else {
				showAlert(false, reason);
			}
		});
	} else {
		showAlert(false, "頻繁發送驗證碼，請稍後再試");
    }

}

// 手機驗證碼驗證  
function verify(cellphone, callbackFun) {
	var verificationNumber = $("#validate_number").val();
	var success = 0;
	console.log(verificationNumber);
	$.post('/Register/verifyPhoneCaptcha', { cellphone: cellphone, captcha: verificationNumber }).done(function (verifyRes) {
		var reason = verifyRes.reason;
		success = verifyRes.success;
		console.log(success);
		if (success === 1) {
			console.log(1);
			showAlert(true, reason);
			callbackFun;
		} else {
			console.log(0);
			showAlert(false, reason);
		}
	});
	return true;
}


// 重置密碼 
function resetPwd(callbackFun) {
	var cellphone = $("#forget_phone").val();
	var newPsw = $("#new_psw").val();
	var rekeyinNewPsw = $("#rekeyin_new_psw").val();
	var success = 0;
	$.post('/Register/resetPassword', { cellphone: cellphone, newPassword: newPsw, reKeyinPassword: rekeyinNewPsw }).done(function (resetRes) {
		var reason = resetRes.reason;
		success = resetRes.success;
		if (success === 1) {
			showAlert(true, reason);
			callbackFun();
		} else {
			showAlert(false, reason);
		}
	});
	return true;
}

function logout() {
	var loginInfo = JSON.parse(localStorage.getItem('loginInfo'));
	$.post('/Login/logout', { token: loginInfo.token }).done(function (logoutRes) {
		localStorage.removeItem('loginInfo');
		showAlert(true, logoutRes.reason, function () { window.location.href = ""; });
	});
}

function loginFlagMethod(loginInfo = null) {
	var loginFlag = false;
	if (loginInfo != null) {
		loginFlag = true;
    }
	return loginFlag;
}

function getWithExpired(key) {
	const itemStr = localStorage.getItem(key);
	// if the item doesn't exist, return null
	if (!itemStr) {
		return null;
	}
	const item = JSON.parse(itemStr);
	const now = new Date();
	// compare the expiry time of the item with the current time
	if (parseInt(now.getTime() / 1000) > item.expired) {
		// If the item is expired, delete the item from storage
		// and return null
		localStorage.removeItem(key);
		return null;
	}
	return item;
}

function refreshSubMenu() {
	localStorage.removeItem('timeId');
	var item = "";
    // 是否過期，如果未過期則取得資料 
	var loginInfo = getWithExpired('loginInfo');
	var loginFlag = loginFlagMethod(loginInfo);
	if (loginFlag) {
		item += `
	    <li><a href="#">語言選擇</a></li>		
	    <li><a class="welcome_text" href="#">親愛的會員：${loginInfo.name}，您好</a></li>
        <li><a class="logout_btn" href="#">登出</a></li>`;
	} else {
		item += `
        <li><a href="#">語言選擇</a></li>
        <li><a class="login_in_btn" href="#">登入</a></li>
        <li><a class="register_btn" href="#">註冊</a></li>`;
	}
	$(".sub_menu").html(item);
}

$("#tab_1,#tab_2,#tab_3,#tab_4").on('click', '#tripInfo', function () {
	console.log("tripInfo");
	var travelCode = $(this).closest("li").find("#travelCode").val();
	let $tab_title = $(this).parents("div").attr("id");
	console.log($tab_title);
	var tabTitleArr = $tab_title.split('_');
	var url = window.location.href;
	var urlArr = url.split('/');
	var indexFlag = (urlArr[urlArr.length - 1] == "" || urlArr[urlArr.length - 1] == "#") ? true : false;
	console.log(indexFlag);
	console.log(urlArr.length);
	console.log(url);
	console.log(travelCode);
	console.log(tabTitleArr[1]);
	tripInfo(travelCode, tabTitleArr[1], indexFlag);
})

// 上傳會員行程預定匯款圖片
function uploadPic(file, picType) {
	var data = new FormData();
	var loginInfo = JSON.parse(localStorage.getItem('loginInfo'));
	var _token = loginInfo.token;
	data.append("file", file[0]);
	data.append("picType", picType);
	data.append("token", _token);
	$.ajax({
		url: "/File/uploadPicToAzure",
		type: "POST",
		data: data,
		dataType: "JSON",
		processData: false,
		contentType: false,
		success: function (data) {
			showAlert(true, data.reason);
			localStorage.setItem("picPath", data.picPath);
		}
	});
}

// 建立旅遊預定匯款證明      
function addReservationCheck(_data = "") {
	alert("111");
	$.ajax({
		url: "/TripReservation/addReservationCheck",
		type: "POST",
		data: JSON.stringify(_data),
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (data) {
			alert(data.reason);
			showAlert(true, data.reason, function () { window.location.href = ""; });
		}
	})
}

// 取消旅遊訂單      
function cancelReservation(_token = "", _travelReservationCode = "") {
	$.post('/Member/cancelMemberReservation', { token: _token, travelReservationCode: _travelReservationCode }).done(function (cancelReservationRes) {
		var reason = cancelReservationRes.reason;
		var success = cancelReservationRes.success;
		if (success == 1) {
			showAlert(true, reason, function () { window.location.href = ""; });
		} else {
			showAlert(false, reason);
		}
	});
}

// 顯示alert  
function showAlert(_success = true, _msg = "", _callback = "") {
	if (_success) {
		$(".alert_success_msg").html(_msg);
		$(".alert.success").fadeTo(0, 500).css("display", "block");
	} else {
		$(".alert_warning_msg").html(_msg);
		$(".alert.warning").fadeTo(0, 500).css("display", "block");
    } 
	window.setTimeout(function () {
		$(".alert").fadeTo(500,0).slideUp(500, function () {
			if (_success) {
				$(".alert.success").css("display", "none");
			} else {
				$(".alert.warning").css("display", "none");
			} 
		});
		if (_callback != "") {
			console.log("_callback");
			_callback();
		}
	}, 1000);
}

function settime(obj, countdown) {
	const startTime = Date.now();
	var timelocalStor = localStorage.getItem("timeId");
	var timeId = null;
	if (timeId == null && timelocalStor == null) { // 如果timer為空 則開啟定時器   
		var timeId = setInterval(function () {
			var runFlag = time(obj, startTime);
			if (!runFlag) {
				clearInterval(timeId);
				timeId = null;  // 將狀態轉為空 
				localStorage.removeItem('timeId');
            }
		}, 1000)
		localStorage.setItem("timeId", timeId);
	}
	console.log(timeId);
}


function time(obj, startTime) {
	var runFlag = true;
	const millis = Date.now() - startTime;
	var sec = Math.floor(millis / 1000);
	var countdown = 60 - sec;
	if (countdown == 0) {
		obj.css("opacity", "1");
		obj.css("pointer-events", "");
		obj.html(`獲取驗證碼`);
		countdown = 60;
		runFlag = false;
		return runFlag;
	} else {
		obj.css("opacity", "0.5");
		obj.css("pointer-events", "none");
		obj.html(`重新傳送(${countdown})`);
		return runFlag;
	}
}

