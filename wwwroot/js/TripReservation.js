

$(function () {
    const $login_modal = $(".login_modal");
    const $mask = $(".mask");
    verifyMemberInfo($login_modal, $mask);

    /**取得旅遊梯次編碼*/
    var _url = window.location.href;
    var arr = _url.replace('#', '').split('/');
    var length = arr.length;
    var _travelStepCode = arr.pop();
    var _travelType = arr[length - 2];
    var _travelCode = _travelStepCode.substring(0, 16);

    localStorage.setItem("travelType", JSON.stringify(_travelType));

    /**取得token*/
    var loginInfoEncode = localStorage.getItem("loginInfo");
    var loginInfo = JSON.parse(loginInfoEncode);
    var _token = null;
    var seatSelectedArr = [];
    var boardingArr = [];

    if (loginInfo != null) {
        _token = loginInfo.token;

        // loading
        console.log("start_loading");
        $mask.show();
        $('body').append('<div style="" id="loadingDiv"><div class="loader">Loading...</div></div>');

        /**loading 頁面時 query API*/
        getReservationBoardingList(_token, _travelCode, boardingArr);
        getReservationSeatInfo(_travelCode, _travelStepCode, _token);
        multipledayTripInfo(_travelCode, _travelStepCode, _travelType);
    }

    /**旅遊梯次選擇 API*/
    $(document).on("click", ".tour_bus_seat", function () {
        console.log("tour_bus_seat");
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

        getReservationFieldInfo(seatSelectedArr);
    });

    // 建立旅遊預定 
    $(document).on("click", ".trip_more_btn", function () {
        console.log("trip_more_btn");
        console.log($(document).find("div.booking_ticket"));
        var bookingTicketTab = $(document).find("div.booking_ticket");
        let dataJson = {};
        let memberReservationArr = [];
        for (let i = 0; i < bookingTicketTab[0].children.length; i++) {
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
            } else {
                roomsType = 0;
            }
            var food = $(children).find("select#food").val();
            var mealsType = 1;
            if (food == "vegetarian_food") {
                mealsType = 2;
            }
            var memo = $(children).find("textarea#memo").val() != null && $(children).find("textarea#memo").val() != "" ? $(children).find("textarea#memo").val() : "";
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
        createReservation(dataJson);
    });

    $(document).on("click", ".multiple_day_travel_list_box ul.tour_bus_choose_box li", function () {
        var hrefTab = $(this).find("a").attr("href");
        var hrefTabArr = hrefTab.split('_');
        const $dayDetailTab = $(this).closest("ul").siblings("#tab_" + hrefTabArr[1]);
        $(".tab_content[class!='tab_content_" + hrefTabArr[1] + "']").hide();
        $(".tab_content_" + hrefTabArr[1]).show();
        $(this).addClass("tab_current selected").siblings().removeClass("tab_current selected");
    });
});

// 取得會員資訊模塊  
function verifyMemberInfo(_loginModal, _mask) {
    // 判斷是否已登入，沒登入則跳回首頁  
    var loginInfo = JSON.parse(localStorage.getItem('loginInfo'));
    var loginFlag = loginFlagMethod(loginInfo);
    if (!loginFlag) {
        _mask.show();
        _loginModal.show();
    }
}



// 定位座位資訊 API 模塊  
function getReservationSeatInfo(_travelCode = "", _travelStepCode = "", _token = "") {
    $.post('/TripReservation/getTripReservationSeatList', { token: _token, travelCode: _travelCode, travelStepCode: _travelStepCode }).done(function (tripReservationSeatInfo) {
        var reservationSeatList = tripReservationSeatInfo.reservationSeatList;
        var reservationSeatContent = "";
        var reservationTransportationContent = "";
        var reservationTransportationTab = "";
        var reservationTransportationContentNew = "";
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
                                <div class="tour_bus_seat_right">
                                    <div class="tour_bus_safe_door">座位車頭區</div>
                                </div>
                                <div class="tour_bus_seat_right">
                                    <div class="tour_bus_safe_door">前門</div>
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

/**定位上車地點資訊 API 模塊*/
function getReservationBoardingList(_token = "", _travelCode = "", boardingArr = []) {
    $.post('/TripReservation/getTripReservationBoardingList', { token: _token, travelCode: _travelCode }).done(function (tripReservationBoardingList) {
        var travelReservationBoardingList = tripReservationBoardingList.travelReservationBoardingList;
        localStorage.setItem("boardingArr", JSON.stringify(travelReservationBoardingList));
    });
}


function createReservation(_data = "") {
    $.ajax({
        url: "/TripReservation/createTravelReservation",
        type: "POST",
        data: JSON.stringify(_data),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            var reason = data.reason;
            var success = data.success;
            alert(data.reason);
            if (success == 0) {
                showAlert(false, reason);
                return;
            } else {
                showAlert(true, reason, function () { window.location.href = "/"; });
            }

        }
    })
}

/**定位座位資訊 API 模塊*/
function getReservationFieldInfo(seatSelectedArr = "") {

    var reservationFieldList = "";
    var reservationBoardingList = "";
    var boardingArr = JSON.parse(localStorage.getItem('boardingArr'));
    var travelType = JSON.parse(localStorage.getItem('travelType'));
    var memberInfo = JSON.parse(localStorage.getItem('memberInfo'));
    var memberId = memberInfo.username;
    var memberName = memberInfo.name;
    var memberBirthday = memberInfo.birthday;
    var memberCellphone = memberInfo.cellphone;

    $.each(boardingArr, function (boardingKey, boardingInfo) {
        var boardingId = boardingInfo.boardingId;
        var boardingLocationName = boardingInfo.boardingLocationName;
        var boardingTime = boardingInfo.boardingTime;
        reservationBoardingList += `
                <option value="${boardingId}">${boardingTime} ${boardingLocationName}</option>`;
    });

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
                        <input type="text" id="personal_name" value=${memberName}>
                    </div>
                </div>
                <div class="booking_list clearfix">
                    <div class="booking_title">
                        <label for="birthday" >出生年月日</label>
                    </div>
                    <div class="booking_field">
                        <input type="date" id="birthday" value=${memberBirthday}>
                    </div>
                </div>
                <div class="booking_list clearfix">
                    <div class="booking_title">
                        <label for="personal_id">身分證</label>
                    </div>
                    <div class="booking_field">
                        <input type="text" id="personal_id" value=${memberId}>
                    </div>
                </div>
                <div class="booking_list clearfix">
                    <div class="booking_title">
                        <label for="telphone">電話</label>
                    </div>
                    <div class="booking_field">
                        <input type="text" id="telphone" value=${memberCellphone}>
                    </div>
                </div>`
        if (travelType != 1) {
            reservationFieldList += `<div class="booking_list clearfix">
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
            </div>`;
        }
        reservationFieldList += `<div class="booking_list clearfix">
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

/**取得多日旅遊詳情 API 模塊*/
function multipledayTripInfo(_travelCode = "", _travelStepCode = "", _travelType = "") {

    switch (_travelType) {
        case "1":
            $("img.banner_icon").attr("src", "/images/banner_singleday_trip_icon.png");
            $(".trip_type_name").attr("href", "/Trip/SingledayTrip");
            $(".page_name").html("一日旅遊");
            break;
        case "2":
            $("img.banner_icon").attr("src", "/images/banner_multiple_day_trip_icon.png");
            $(".trip_type_name").attr("href", "/Trip/MultipledayTrip");
            $(".page_name").html("多日旅遊");
            break;
        case "3":
            $("img.banner_icon").attr("src", "/images/banner_island_trip_icon.png");
            $(".trip_type_name").attr("href", "/Trip/IslandTrip");
            $(".page_name").html("離島旅遊");
            break;
        case "4":
            $("img.banner_icon").attr("src", "/images/banner_car_trip_icon.png");
            $(".trip_type_name").attr("href", "/Trip/CarTrip");
            $(".page_name").html("包車旅遊");
            break;
        case "5":
            $("img.banner_icon").attr("src", "/images/banner_foreign_trip_icon.png");
            $(".trip_type_name").attr("href", "/Trip/ForeignTrip");
            $(".page_name").html("國外旅遊");
            break;
    }
    $.post('/Trip/GetTravelInfoForMember', { travelCode: _travelCode, travelStepCode: _travelStepCode }).done(function (tripInfo) {
        var travelTitle = tripInfo.travelInfo.travelTitle;
        var costInfo = tripInfo.costInfo;
        var dateReducedTravelItem = "";
        var selectedTravelStep = "";
        var costInfoHhtml = "行程費用: 團費含: <br>" +
            (costInfo.actionInfo == "" ? "" : (costInfo.actionInfo + "<br>")) +
            (costInfo.eatInfo == "" ? "" : (costInfo.eatInfo + "<br>")) +
            (costInfo.nearInfo == "" ? "" : (costInfo.nearInfo + "<br>")) +
            (costInfo.transportationInfo == "" ? "" : (costInfo.transportationInfo + "<br>")) +
            (costInfo.insuranceInfo == "" ? "" : (costInfo.insuranceInfo + "<br>")) +
            (costInfo.liveInfo == "" ? "" : "" + (costInfo.liveInfo));
        console.log(costInfo.liveInfo);
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

        $.each(tripInfo.travelStatisticList, function (dateIndex, travelStatisticInfo) {
            $.each(travelStatisticInfo.travelStatisticList, function (travelStepIndex, travelStepStatisticInfo) {
                var selectFlag = travelStepStatisticInfo.travelStepSelectFlag;
                if (selectFlag == 1) {
                    selectedTravelStep = travelStepStatisticInfo;
                }
            });
        });

        var travelSdate = selectedTravelStep.startDate;
        var travelEdate = selectedTravelStep.endDate;
        var sDay = new Date(Date.parse(travelSdate.replace(/-/g, '/'))).getDay();
        var eDay = new Date(Date.parse(travelEdate.replace(/-/g, '/'))).getDay();

        $(".travel_title").html(travelTitle);
        $(".banner_title").html(travelTitle);
        $(".bread_title").html(travelTitle);
        $(".travel_sdate").html(travelSdate + `(${dayList[sDay]})`);
        $(".travel_edate").html(travelEdate + `(${dayList[eDay]})`);
        $(".travel_info").html(dateReducedTravelItem);
        $(".cost_info").html(costInfoHhtml);

        $mask.hide();
        removeLoader();
        console.log("finish_loading");
    });
}

function removeLoader() {
    $("#loadingDiv").fadeOut(500, function () {
        // fadeOut complete. Remove the loading div
        $("#loadingDiv").remove(); //makes page more lightweight 
    });
}
