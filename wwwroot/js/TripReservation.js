

$(function () {
    const $login_modal = $(".login_modal");
    const $mask = $(".mask");
    verifyMemberInfo($login_modal, $mask);

    // 取得旅遊梯次編碼    
    var _url = window.location.href;
    var paramStr = _url.split('?')[1].split('=');
    var _travelStepCode = paramStr[1];
    var _travelCode = _travelStepCode.substring(0, 16);
    console.log(_url);

    // 取得token 
    var loginInfoEncode = localStorage.getItem("loginInfo");
    var loginInfo = JSON.parse(loginInfoEncode);
    var _token = loginInfo.token;
    var seatSelectedArr = [];
    var boardingArr = [];

    // loading 頁面時 query API
    getReservationBoardingList(_token, _travelCode, boardingArr);
    getReservationSeatInfo(_travelCode, _travelStepCode, _token);
    multipledayTripInfo(_travelCode, _travelStepCode);

    // 旅遊梯次選擇    
    $(document).on("click", ".tour_bus_seat", function () {
        console.log("tour_bus_seat");
        console.log(boardingArr);
        var seat = $(this).find("input").val();
        var className = $(this).attr('class');
        if (!className.includes("booking_seat")) {
            $(this).addClass("booking_seat");
            if (seatSelectedArr.indexOf(seat) == -1) {
                seatSelectedArr.push(seat);
            }
        } else {
            $(this).removeClass("booking_seat");
            var seatIndex = seatSelectedArr.indexOf(seat);
            seatSelectedArr.splice(seatIndex, 1);
        }

        getReservationFieldInfo(seatSelectedArr, boardingArr);
        console.log(seatSelectedArr);
    });

    // 建立旅遊預定 
    $(document).on("click", ".trip_more_btn", function () {
        console.log("trip_more_btn");
        console.log($(document).find("div.booking_ticket"));
        var bookingTicketTab = $(document).find("div.booking_ticket");
        let dataJson = {};
        let memberReservationArr = [];
        for (let i = 0; i < bookingTicketTab[0].children.length; i++) {
            console.log(bookingTicketTab[0].children.length);
            console.log(i);
            var children = bookingTicketTab[0].children[i];
            var id = $(children).find("input#personal_id").val();
            var seatId = $(children).find("input#seatId").val();
            var transportationId = $(children).find("input#transportationId").val();
            var name = $(children).find("input#personal_name").val();
            var phone = $(children).find("input#telphone").val();
            var birthday = $(children).find("input#birthday").val();
            var boardingId = $(children).find("select#tour_bus_take").val();
            var room = $(children).find("select#room").val();
            var roomsType = 1;
            var memberReservation = {};
            if (room == "two_room") {
                roomsType = 2;
            } else if (room == "three_room") {
                roomsType = 3;
            } else if (room == "four_room") {
                roomsType = 4;
            }
            var food = $(children).find("select#food").val();
            var mealsType = 1;
            if (food == "vegetarian_food") {
                mealsType = 2;
            }
            var memo = $(children).find("textarea#memo").val() != null && $(children).find("textarea#memo").val() != "" ? $(children).find("textarea#memo").val() : "";
            console.log(seatId);
            console.log(id);
            console.log(name);
            console.log(phone);
            console.log(birthday);
            console.log(boardingId);
            console.log(mealsType);
            console.log(roomsType);
            console.log(memo);
            console.log(seatId);
            console.log(transportationId);
            memberReservation["id"] = id;
            memberReservation["name"] = name;
            memberReservation["phone"] = phone;
            memberReservation["birthday"] = birthday;
            memberReservation["mealsType"] = mealsType;
            memberReservation["roomsType"] = roomsType;
            memberReservation["boardingId"] = parseInt(boardingId);
            memberReservation["transportationId"] = parseInt(transportationId);
            memberReservation["seatId"] = parseInt(seatId);
            memberReservation["note"] = memo;
            memberReservationArr.push(memberReservation);
        }
        dataJson['travelStepCode'] = _travelStepCode;
        dataJson['token'] = _token;
        dataJson['memberReservationArr'] = memberReservationArr;
        console.log(dataJson);
        createReservation(dataJson);
    });

    // Tab切換 
    $(document).on("click", ".multiple_day_travel_list_box ul.tour_bus_choose_box li", function () {
        console.log("tour_bus_choose_box > li");
        var hrefTab = $(this).find("a").attr("href");
        console.log(hrefTab);
        var hrefTabArr = hrefTab.split('_');
        const $dayDetailTab = $(this).closest("ul").siblings("#tab_" + hrefTabArr[1]);
        console.log("tab_content" + hrefTabArr[1]);
        $(".tab_content[class!='tab_content_" + hrefTabArr[1] + "']").hide();
        console.log($dayDetailTab.attr("style"));
        $(".tab_content_" + hrefTabArr[1]).show();
        console.log($dayDetailTab.attr("style"));
        $(this).addClass("tab_current selected").siblings().removeClass("tab_current selected");
    });
});

// 取得會員資訊模塊   
function verifyMemberInfo(_loginModal, _mask) {
    
    // 判斷是否已登入，沒登入則跳回首頁  
    var loginInfo = JSON.parse(localStorage.getItem('loginInfo'));
    var loginFlag = loginFlagMethod(loginInfo);
    console.log(loginFlag);
    if (!loginFlag) {
        _mask.show();
        _loginModal.show();
    }
}



// 定位座位資訊 API 模塊  
function getReservationSeatInfo(_travelCode = "", _travelStepCode = "", _token = "") {
    $.post('/TripReservation/getTripReservationSeatList', { token: _token, travelCode: _travelCode, travelStepCode: _travelStepCode }).done(function (tripReservationSeatInfo) { 
        var reservationSeatList = tripReservationSeatInfo.reservationSeatList; // 定位座位列表 
        var reservationSeatContent = "";  // 定位座位資訊
        var reservationTransportationContent = "";   // 定位車次資訊
        var reservationTransportationTab = "";   // 定位車次標籤資訊 
        var reservationTransportationContentNew = "";   // 定位車次標籤資訊
        var transportationIdArr = [];

        reservationTransportationTab += `<ul class="tour_bus_choose_box clearfix">`;
        $.each(reservationSeatList, function (reservationSeatIndex, reservationSeatInfo) {
            var seatList = reservationSeatInfo.reservationSeatList;
            var useStatus = reservationSeatInfo.useStatus;
            var transportationStep = reservationSeatInfo.transportationStep;
            var transportationId = reservationSeatInfo.transportationId;

            if (useStatus == 1) {
                if (transportationIdArr.length == 0) {
                    transportationIdArr.push(transportationStep);
                    reservationTransportationTab += `<li class="tab-link tour_bus_seat_btn tab_current"><a href="#tab_${transportationStep}">遊覽車車次${transportationStep}</a></li>`;
                    reservationSeatContent += `<div id="tab_${transportationStep}" class="tour_bus_seat_map tab_content tab_content_${transportationStep} tour_bus_seat_current clearfix" style="display: block">`;
                } else {
                    reservationTransportationTab += `<li class="tab-link tour_bus_seat_btn"><a href="#tab_${transportationStep}">遊覽車車次${transportationStep}</a></li>`;
                    reservationSeatContent += `<div id="tab_${transportationStep}" class="tour_bus_seat_map tab_content tab_content_${transportationStep} clearfix" style="display: none">`;
                }
                
                // 整理車次 
                reservationTransportationContent += `<li class="booking_list_title">車次</li>`;
                reservationTransportationContent += `<li>第${transportationStep}車</li>`;
                
                // 定位座位資訊  
                $.each(seatList, function (seatIndex, seatInfo) {
                    var seatId = seatInfo.seatId;
                    var seatName = seatInfo.seatName;
                    var seatStatus = seatInfo.seatStatus;
                    if (seatIndex < 24) {
                        if (seatIndex == 0) {
                            reservationSeatContent += `<div class="tour_bus_seat_left_box">`;
                        }
                        if (seatIndex % 2 == 0) {
                            reservationSeatContent += `<div class="tour_bus_seat_left clearfix">`;
                        }
                        if (seatStatus == 0) {
                            reservationSeatContent += `<div class="tour_bus_seat disabled_seat">${seatName}<input type="hidden" id="seatId" value="${transportationStep}:${transportationId}:${seatId}:${seatName}"></div>`;
                        } else {
                            reservationSeatContent += `<div class="tour_bus_seat">${seatName}<input type="hidden" id="seatId" value="${transportationStep}:${transportationId}:${seatId}:${seatName}"></div>`;
                        }
                        if (seatIndex % 2 == 1) {
                            reservationSeatContent += `</div>`;
                        }
                        if (seatIndex == 23) {
                            reservationSeatContent += `</div>`;
                        }
                    } else if (seatIndex >= 24 && seatIndex < 43) {
                        if (seatIndex == 24) {
                            reservationSeatContent += `<div class="tour_bus_seat_right_box">
                                <div class="tour_bus_seat_right clearfix">
                                    <div class="tour_bus_seat no_seat">右0排道</div>
                                    <div class="tour_bus_seat no_seat">右0排窗</div>
                                </div>
                                <div class="tour_bus_seat_right clearfix">
                                    <div class="tour_bus_seat no_seat">右0排道</div>
                                    <div class="tour_bus_seat no_seat">右0排窗</div>
                                </div>`;
                        }
                        if (seatIndex % 2 == 0) {
                            if (seatIndex == 34) {
                                reservationSeatContent += `
                               <div class="tour_bus_seat_right">
                                     <div class="tour_bus_safe_door">後門</div>
                               </div>`;
                            }
                            reservationSeatContent += `<div class="tour_bus_seat_right clearfix">`;
                        }
                        if (seatStatus == 0) {
                            reservationSeatContent += `<div class="tour_bus_seat disabled_seat">${seatName}<input type="hidden" id="seatId" value="${transportationStep}:${transportationId}:${seatId}:${seatName}"></div>`;
                        } else {
                            reservationSeatContent += `<div class="tour_bus_seat">${seatName}<input type="hidden" id="seatId" value="${transportationStep}:${transportationId}:${seatId}:${seatName}"></div>`;
                        }
                        if (seatIndex % 2 == 1) {
                            reservationSeatContent += `</div>`;
                        }
                        if (seatIndex == 42) {
                            reservationSeatContent += `</div>`;
                        }
                    }
                });
                reservationSeatContent += `</div></div>`;
            }    
        });
        reservationTransportationTab += `</ul>`;
        reservationTransportationContentNew += reservationTransportationTab;
        reservationTransportationContentNew += reservationSeatContent;
        reservationTransportationContentNew += `<div class="booking_ticket"></div>`;
        $(".multiple_day_travel_list_box").html(reservationTransportationContentNew);
        $(".travel_transportation").html(reservationTransportationContent);
    });
}

// 定位上車地點資訊 API 模塊 
function getReservationBoardingList(_token = "", _travelCode = "", boardingArr = []) {
    $.post('/TripReservation/getTripReservationBoardingList', { token: _token, travelCode: _travelCode }).done(function (tripReservationBoardingList) {
        var travelReservationBoardingList = tripReservationBoardingList.travelReservationBoardingList; // 定位上車地點資訊
        localStorage.setItem("boardingArr", JSON.stringify(travelReservationBoardingList));
    });
}





// 建立旅遊預約      
function createReservation(_data = "") {
    $.ajax({
        url: "/TripReservation/createTravelReservation",
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

// 定位座位資訊 API 模塊    
function getReservationFieldInfo(seatSelectedArr = "") {

    var reservationFieldList = ""; // 預約欄位列表
    var reservationBoardingList = ""; // 預約上車列表
    var boardingArr = JSON.parse(localStorage.getItem('boardingArr'));

    // 定位座位資訊 
    $.each(boardingArr, function (boardingKey, boardingInfo) {
        var boardingId = boardingInfo.boardingId;
        var boardingLocationName = boardingInfo.boardingLocationName;
        var boardingTime = boardingInfo.boardingTime;
        reservationBoardingList += `
                <option value="${boardingId}">${boardingTime} ${boardingLocationName}</option>`;
    });

    // 定位座位資訊 
    $.each(seatSelectedArr, function (seatSelectedKey, seatSelected) {
        var seatSelectedInfoArr = seatSelected.split(':');
        var transportationStep = seatSelectedInfoArr[0];
        var transportationId = seatSelectedInfoArr[1];
        var seatId = seatSelectedInfoArr[2];
        var seatName = seatSelectedInfoArr[3];
        reservationFieldList += `      
            <div class="booking_table_content">
                <div class="booking_seat">第${transportationStep}車 ${seatName}</div>
                <input type="hidden" id="seatId" value="${seatId}">
                <input type="hidden" id="transportationId" value="${transportationId}">
                <div class="booking_list clearfix">
                    <div class="booking_title"><label for="tour_bus_take">上車地點</label></div>
                    <div class="booking_field">
                        <select id="tour_bus_take">
                        ${reservationBoardingList}
                        </select>
                    </div>
                </div>
                <div class="booking_list clearfix">
                    <div class="booking_title">
                        <label for="personal_name">姓名</label>
                    </div>
                    <div class="booking_field">
                        <input type="text" id="personal_name">
                    </div>
                </div>
                <div class="booking_list clearfix">
                    <div class="booking_title">
                        <label for="birthday">出生年月日</label>
                    </div>
                    <div class="booking_field">
                        <input type="date" id="birthday">
                    </div>
                </div>
                <div class="booking_list clearfix">
                    <div class="booking_title">
                        <label for="personal_id">身分證</label>
                    </div>
                    <div class="booking_field">
                        <input type="text" id="personal_id">
                    </div>
                </div>
                <div class="booking_list clearfix">
                    <div class="booking_title">
                        <label for="telphone">電話</label>
                    </div>
                    <div class="booking_field">
                        <input type="text" id="telphone">
                    </div>
                </div>
                <div class="booking_list clearfix">
                    <div class="booking_title">
                        <label for="food">房型</label>
                    </div>
                    <div class="booking_field">
                        <select id="room">
                            <option value="two_room">二人房</option>
                            <option value="three_room">三人房</option>
                            <option value="four_room">四人房</option>
                        </select>
                    </div>
                </div>
                <div class="booking_list clearfix">
                    <div class="booking_title">
                        <label for="food">葷食/素食</label>
                    </div>
                    <div class="booking_field">
                        <select id="food">
                            <option value="meat_food">葷食</option>
                            <option value="vegetarian_food">素食</option>
                        </select>
                    </div>
                </div>
                <div class="booking_list clearfix">
                    <div class="booking_title"><label for="memo">備註</label></div>
                    <div class="booking_field"><textarea id="memo"></textarea></div>
                </div>
            </div>`;
    });
    $(".booking_ticket").html(reservationFieldList);
}

// 取得多日旅遊詳情 API 模塊 
function multipledayTripInfo(_travelCode = "", _travelStepCode = "") {
    console.log(_travelCode);
    console.log(_travelStepCode);
    $.post('/Trip/GetTravelInfoForMember', { travelCode: _travelCode, travelStepCode: _travelStepCode }).done(function (tripInfo) {
        var travelTitle = tripInfo.travelInfo.travelTitle;  // 旅遊標題
        var dateReducedTravelItem = "";   // 精簡模式資訊  
        var selectedTravelStep = "";   // 已被選擇旅遊梯次資訊
        var dayList = ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六'];

        // 整理旅遊精簡模式資訊   
        dateReducedTravelItem += `<li class="booking_list_title">行程內容</li>`;
        $.each(tripInfo.dateTravelPicList, function (dateIndex, dateTravelPicInfo) {
            var travelDay = "第" + (dateIndex + 1) + "天";
            var travelPicTitle = "";
            $.each(dateTravelPicInfo.travelPicList, function (picIndex, travelPicInfo) {
                if (picIndex == (dateTravelPicInfo.travelPicList.length - 1)) {
                    travelPicTitle += travelPicInfo.travelPicTraditionalTitle;
                } else {
                    travelPicTitle += travelPicInfo.travelPicTraditionalTitle + "、";
                }
            });
            dateReducedTravelItem += "<li>" + travelDay + ": " + travelPicTitle + "</li>";
        });

        // 整理旅遊日期資訊  
        $.each(tripInfo.travelStatisticList, function (dateIndex, travelStatisticInfo) {
            $.each(travelStatisticInfo.travelStatisticList, function (travelStepIndex, travelStepStatisticInfo) {
                var selectFlag = travelStepStatisticInfo.travelStepSelectFlag;
                if (selectFlag == 1) {
                    selectedTravelStep = travelStepStatisticInfo;
                }
            });
        });
        var travelSdate = selectedTravelStep.startDate;
        console.log(travelSdate);
        var travelEdate = selectedTravelStep.endDate;
        console.log(travelEdate);
        var sDay = new Date(Date.parse(travelSdate.replace(/-/g, '/'))).getDay();
        var eDay = new Date(Date.parse(travelEdate.replace(/-/g, '/'))).getDay();

        $(".travel_title").html(travelTitle);
        $(".banner_title").html(travelTitle);
        $(".bread_title").html(travelTitle);
        $(".travel_sdate").html(travelSdate + `(${dayList[sDay]})`);
        $(".travel_edate").html(travelEdate + `(${dayList[eDay]})`);
        $(".travel_info").html(dateReducedTravelItem);
    });
}
