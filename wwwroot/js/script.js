// JavaScript Document
$(function () {
	const $header_box = $(".header_box").offset().top;
	const $policy_btn = $(".policy_btn");
	const $rule_btn = $(".rule_btn");
	const $banner_text_h1 = $(".banner_text").find("h1");
	const $banner_icon = $("img.banner_icon");
	const $bread_page_name = $(".bread_crumb").find("span.page_name");
	const $mask = $(".mask");
	const $login_modal = $(".login_modal");
	const $register_member_modal = $(".register_member_modal");
	const $video_modal = $(".video_modal");

	//Modal
	$(".login_in_btn").on("click",function(){
		$mask.show();
		$login_modal.show();
	});
	$(".member_login_btn").on("click",function(){
		$(".add_member_modal,.register_member_modal").hide();
		$(".member_number_modal").show();		
	});
	$(".forget_psw").on("click",function(){
		$login_modal.hide();
		$(".forget_psw_modal").show();
	});
	$(".modal_close_icon").on("click",function(){	
		$(".login_modal,.forget_psw_modal,.register_member_modal,.add_member_modal").hide();
		$mask.hide();
	});
	$("a.register_btn").on("click",function(){
		$mask.show();
		$register_member_modal.show();
	});
	$("a.add_member").on("click",function(){
		$register_member_modal.show();
	});
	$("a.agree_btn").on("click",function(){
		$(".add_member_modal").show();
	});
	$(".disagree_btn").on("click",function(){
		$(".login_modal,.register_member_modal").hide();
		$mask.hide();
	})
    $(".photo_video,img.multiple_day_video_photo").on("click", function(){
		$mask.show();
		$video_modal.show();
	});
    $(".video_close_model").on("click", function(){
		$video_modal.hide();
		$mask.hide();
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
				//$banner_text_h1.text("多日旅遊");
				//$banner_icon.attr("src","~/images/banner_multiple_day_trip_icon.png");
				//$bread_page_name.text("多日旅遊");

				$("#tab_1").show();
				break;
			case "island_trip":
				multipledayTripList(3, "tab_2");
				//$banner_text_h1.text("離島旅遊");
				//$banner_icon.attr("src","~/images/banner_island_trip_icon.png");
				//$bread_page_name.text("離島旅遊");

				$("#tab_2").show();
				break;
			case "car_trip":
				multipledayTripList(4, "tab_3");
				//$banner_text_h1.text("包車旅遊");
				//$banner_icon.attr("src","~/images/banner_car_trip_icon.png");
				//$bread_page_name.text("包車旅遊");

				$("#tab_3").show();
				break;
			case "foreign_trip":
				multipledayTripList(5, "tab_4");
				//$banner_text_h1.text("國外旅遊");
				//$banner_icon.attr("src","~/images/banner_foreign_trip_icon.png");
				//$bread_page_name.text("國外旅遊");

				$("#tab_4").show();
				break;
		}
		return false;
	});
	$("ul.two_day_tabs > li,ul.multiple_day_travel_list_tabs > li").on("click",function(){
		let $now_tab = $(this).find("a").attr("href");
		$(".tab_content[class!='tab_content_1']").hide();
		$(this).addClass("two_day_current").siblings().removeClass("two_day_current");
		$($now_tab).fadeIn(300);
		return false;
	});
	$("ul.rule_tabs > li").on("click",function(){
		let $now_tab = $(this).find("a").attr("href");
		$(".tab_content[class!='tab_content_1']").hide();
		$(this).addClass("rule_current").siblings().removeClass("rule_current");
		$($now_tab).fadeIn(300);
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
	$(".tabs_brief_btn").on("click",function(){
		$(".timeline_travel,.travel_table").toggle();
	});

	//Slider
	$(".index_slider").slick({
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

	//Go_Top
	$("#go_top_btn").on("click",function(){
		$("html,body").stop().animate({
			scrollTop: $header_box
		}, 800);
		return false;
	});
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
                    <img src="${trip.travelPicPath}" alt="台北近郊一日遊" title="台北近郊一日遊">
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