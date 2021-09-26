// JavaScript Document 
$(function () {
	const $banner_place = $(".banner_place");
	const $header_box = $(".header_box").offset().top;
	const $policy_btn = $(".policy_btn");
	const $rule_btn = $(".rule_btn");
	const $banner_text_h1 = $(".banner_text").find("h1");
	const $banner_icon = $("img.banner_icon");
	const $bread_page_name = $(".bread_crumb").find("span.page_name");
	const $mask = $(".mask");
	const $login_modal = $(".login_modal");
	const $register_member_modal = $(".register_member_modal");
	const $phone_verification_modal = $(".phone_verification_modal");
	const $add_member_modal = $(".add_member_modal");
	const $video_modal = $(".video_modal");
	const $upload_pay_modal = $(".upload_pay_modal");
	const $fixed_btn_box = $(".fixed_btn_box");
	refreshSubMenu();

	//Modal 
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
		$(".forget_psw_modal").show();
	});
	$(".modal_close_icon").on("click", function () {
		$(".login_modal,.forget_psw_modal,.register_member_modal,.add_member_modal,.phone_verification_modal,.upload_pay_modal").hide();
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
		console.log(travelReservationCode);
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
		console.log(picPath);
		let dataJson = {};
		var bankAccountCode = $(this).closest("div").find("input#bank_number").val();
		console.log(bankAccountCode);
		dataJson['token'] = _token;
		dataJson['travelReservationCode'] = travelReservationCode;
		dataJson['bankAccountCode'] = bankAccountCode;
		dataJson['travelReservationCheckPicPath'] = picPath;
		console.log(dataJson);
		addReservationCheck(dataJson);
		$upload_pay_modal.hide();
		$mask.hide();
	}); 
	$(document).on("click", ".choose_photo_file_btn", function () {
		console.log("choose_photo_file_btn");
		var file = $("#upload_pay_photo").get(0).files;
		console.log(file);
	});
	$("#upload_pay_photo").change(function () {
		var file = $("#upload_pay_photo").get(0).files;
		console.log(file);
		uploadPic(file, 1);
	});



	$("ul.travel_tabs").find("li").on("click",function(){
		let $travel_tab = $(this).find("a").attr("href");
		let $travel_tab_title = $(this).attr("id");
		$(".tab_content[class!='tab_content_1']").hide();
		$(this).addClass("travel_btn_current").siblings().removeClass("travel_btn_current");
		$($travel_tab).fadeIn(300);
		switch($travel_tab_title){
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
	$("ul.two_day_tabs > li,ul.multiple_day_travel_list_tabs > li").on("click", function () {
		console.log("2222");
		let $now_tab = $(this).find("a").attr("href");
		$($now_tab).fadeIn(300);
		$(".tab_content[class!='tab_content_1']").hide();
		$(this).addClass("two_day_current").siblings().removeClass("two_day_current");
		return false;
	});
	$("ul.month_choose > li").on("click", function () {
		console.log("1111");
		let $month_tab = $(this).find("a").attr("href");
		$($month_tab).fadeIn(300);
		console.log($month_tab);
		$(".month_content[class!='month_content_1']").hide();
		$(this).addClass("two_day_current").siblings().removeClass("two_day_current");
		return false;
	});
	$("ul.two_day_tabs li:last").on("click", function () {
		$("ul.two_day_tabs").find("li").removeClass("two_day_current");
		$(this).addClass("two_day_current");
		window.location.replace("https://docs.google.com/forms/d/1-usgHugOctshpg8lj9xrSMe9mCax97DGKXF-Yh1Dmsg/edit?hl=zh-tw");
	});
	$("ul.rule_tabs > li").on("click",function(){
		let $now_tab = $(this).find("a").attr("href");
		$($now_tab).fadeIn(300);
		$(".tab_content[class!='tab_content_1']").hide();
		$(this).addClass("rule_current").siblings().removeClass("rule_current");
		if($policy_btn.hasClass("rule_current")){
			$policy_btn.find("img").attr("src","images/arrow_icon_orange"+".png");
		}else{
			$policy_btn.find("img").attr("src","images/arrow_icon_blue"+".png");
		}
		if($rule_btn.hasClass("rule_current")){
			$rule_btn.find("img").attr("src","images/arrow_icon_orange"+".png");
		}else{
			$rule_btn.find("img").attr("src","images/arrow_icon_blue"+".png");
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
				breakpoint: 1024,
				settings: {
					slidesToShow: 4,
					slidesToScroll: 4,
					infinite: true,
					dots: true
				}
			},
			{
				breakpoint: 600,
				settings: {
					slidesToShow: 2,
					slidesToScroll: 2
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


	//Media Icon
	$(window).on("scroll", function () {
		if ($(this).scrollTop() >= $banner_place.offset().top - $banner_place.outerHeight() / 5) {
			$fixed_btn_box.fadeIn(700);
		} else {
			$fixed_btn_box.fadeOut(700);
		}
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
		login();
		$mask.hide();
		$login_modal.hide();
	});

	$(".logout_btn").on("click", function () {
		logout();
	});

	$(".add_member_btn").on("click", function () {
		register($add_member_modal, $phone_verification_modal);
	});

	$(".submit_verification_btn").on("click", function () {
		verify(closePhoneVerificationModel);
	});

	$(".resend_verification_btn").on("click", function () {
		sendVerificationNumber();
	});

	function closePhoneVerificationModel() {
		$mask.hide();
		$phone_verification_modal.hide();
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


function multipledayTripInfo(travelCode = null) {
	var travelCode = travelCode;
	window.location.href = 'MultipledayTripInfo?travelCode=' + travelCode;
}

function login() {
	const now = new Date();
	var number = $("#number").val();
	var psw = $("#psw").val();
	$.post('/Login/login', { username: number, password: psw }).done(function (loginRes) {
		var exp = parseInt(now.getTime() / 1000) + 3600;
		console.log(exp);
		loginRes.data.expired = exp;
		localStorage.setItem("loginInfo", JSON.stringify(loginRes.data));
		window.location.href = "";
	});
}

// 註冊
function register(add_member_modal, phone_verification_modal) {
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
	$.post('/Register/registerByPhone', { username: number, password: password, name: name, email: email, address: address, birthday: birthday, telephone: telephone, cellphone: cellphone, emerContactName: emerContactName, emerContactPhone: emerContactPhone}).done(function (registerRes) {
		var reason = registerRes.reason;
		success = registerRes.success;
		if (success == 1) {
			add_member_modal.hide();
			phone_verification_modal.show();
		}
		alert(reason);
	});
	return true;
}

// 重送驗證碼
function sendVerificationNumber() {
	var cellphone = $("#phone").val();
	$.post('/Register/getPhoneCaptcha', { cellphone: cellphone }).done(function (sendVerificationNumberRes) {
		alert("已重送驗證碼");
	});
}

// 手機驗證碼驗證
function verify(closePhoneVerificationModel) {
	var cellphone = $("#phone").val();
	var verificationNumber = $("#verification_number").val();
	$.post('/Register/verifyPhoneCaptcha', { cellphone: cellphone, captcha: verificationNumber }).done(function (verifyRes) {
		var reason = verifyRes.reason;
		var success = verifyRes.success;
		alert(reason);
		if (success == 1) {
			closePhoneVerificationModel();
		}
	});
	return true;
}

function logout() {
	var loginInfo = JSON.parse(localStorage.getItem('loginInfo'));
	$.post('/Login/logout', { token: loginInfo.token }).done(function (logoutRes) {
		localStorage.removeItem('loginInfo');
		window.location.href = "";
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
	console.log(itemStr);
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

$("#tab_1,tab_2,tab_3,tab_4").on('click', '#tripInfo', function () {
	var travelCode = $(this).closest("li")
		.find("#travelCode")
		.val();
	multipledayTripInfo(travelCode);
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
		url: "/File/uploadPic",
		type: "POST",
		data: data,
		dataType: "JSON",
		processData: false,
		contentType: false,
		success: function (data) {
			alert(data.reason);
			localStorage.setItem("picPath", data.picPath);
		}
	});
}

// 建立旅遊預約      
function addReservationCheck(_data = "") {
	$.ajax({
		url: "/TripReservation/addReservationCheck",
		type: "POST",
		data: JSON.stringify(_data),
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (data) {
			alert(data.reason);
			window.location.href = "";
		}
	})
}

