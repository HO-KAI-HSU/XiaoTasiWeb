

$(function () {

    // 取得旅遊編碼 
    var _url = window.location.href;
    var paramStr = _url.split('?')[1].split('=');
    var travelCode = paramStr[1];
    console.log(_url);

    // loading 頁面時 query API   
    multipledayTripInfo(travelCode);

    // 取得video API 模塊 
    videoList();
        

    // 按鈕更新旅遊統計    
    $(".month_travel_list").on("click", function () {
        console.log("11");
    });

    // Tab切換   
    $(document).on("click", ".calendar_list ul.month_choose li", function () {
        console.log("month_choose > li");
        var hrefTab = $(this).find("div").attr("class");
        var hrefTabArr = hrefTab.split('_');
        const $dayDetailTab = $(this).closest("ul").siblings("#month_choose_tab_" + hrefTabArr[2]);
        $($dayDetailTab).fadeIn(300);
        console.log("month_content_" + hrefTabArr[2]);
        $(".month_content[class!='month_content_" + hrefTabArr[2] + "']").hide();
        console.log($dayDetailTab.attr("style"));
        $(".month_content_" + hrefTabArr[2]).show();
        console.log($dayDetailTab.attr("style"));
        $(this).addClass("two_day_current selected").siblings().removeClass("two_day_current selected");
        $inputTab = $(".month_content_" + hrefTabArr[2]).find("div.month_travel_list.selected input");
        var travelStepCode = $inputTab.val();
        console.log(travelStepCode);
        updateTripStatisticInfo(travelStepCode);
    });

    // 旅遊梯次選擇  
    $(document).on("click", ".calendar_list .month_travel_list", function () {
        console.log("month_travel_list");
        $(this).addClass("selected").siblings().removeClass("selected");
        var travelStepCode = $(this).find("input").val();
        console.log(travelStepCode);
        updateTripStatisticInfo(travelStepCode);
    });

    // 報名   
    $(document).on("click", ".trip_more_btn", function () {
        console.log("trip_more_btn");
        console.log($(document).find("input#travel_step_code").find("input#travel_step_code").val());
        const hrefTab = $(document).find("li.two_day_current").find("div").attr("class");
        console.log(hrefTab);
        var hrefTabArr = hrefTab.split('_');
        $inputTab = $(".month_content_" + hrefTabArr[2]).find("div.month_travel_list.selected input");
        var travelStepCode = $inputTab.val();
        console.log(travelStepCode);
        top.location.href = "/TripReservation/TripReservation?travelStepCode=" + travelStepCode;
    });
});


// 取得多日旅遊詳情 API 模塊 
function multipledayTripInfo(_travelCode = "", _travelStepCode = "") {
    $.post('/Trip/GetTravelInfoForMember', { travelCode: _travelCode, travelStepCode: _travelStepCode}).done(function (tripInfo) {
        var dateTravelItem = "";          // 每日旅遊資訊
        var dateReducedTravelItem = "";   // 精簡模式資訊
        var dateTravelCostItem = "";      // 費用資訊   
        var travelTitleItem = "";         // 整理旅遊標題資訊  
        var travelStatisticsItem = "";    // 整理統計資訊
        var travelStatisticsMonItem = "";    // 整理統計月份資訊  
        var travelMonStatisticsItem = "";    // 整理月份統計資訊
        var travelContentItem = "";       // 整理旅遊內容資訊   
        var travelTitle = tripInfo.travelInfo.travelTitle;
        var travelContent = tripInfo.travelInfo.travelContent;

        // 整理旅遊標題資訊  
        travelTitleItem += `<h3>${travelTitle}<span class="video_mark">世界級的風景饗宴</span></h3>`;
        bannerTitle = `${travelTitle}`;
        breadTitle = `${travelTitle}`;

        // 整理旅遊內容資訊
        travelContentItem += `${travelContent}`;

        // 整理旅遊精簡模式資訊      
        dateReducedTravelItem +=` 
                    <tr>
                        <th class="order_date">天數</th>
                        <th class="place_food_hotel">旅遊行程/景點</th>
                        <th class="place_food_hotel">餐食</th>
                        <th class="place_food_hotel">旅館</th>
                    </tr>`;

        // 整理旅遊費用數說明資訊 
        dateTravelCostItem += `
                    <div class="tabs_fee_text">
                        <h3>費用說明</h3>
                        <p>1.車資：${tripInfo.costInfo.transportationInfo}</p>
                        <p>2.餐食：${tripInfo.costInfo.eatInfo}</p>
                        <p>3.住宿：${tripInfo.costInfo.liveInfo}</p>
                        <p>4.活動：${tripInfo.costInfo.actionInfo}</p>
                        <p>5.貼心：${tripInfo.costInfo.insuranceInfo}</p>
                        <p>6.保險：${tripInfo.costInfo.nearInfo}</p>
                        <p class="tabs_fee_text_notice">＊旅客未滿15歲或70歲以上，依法限制最高【意外死殘保額新臺幣20萬元、意外醫療保額新臺幣20萬 (實支實付)】</p>
                    </div>
                    <div class="tabs_fee_text">
                        <h3>費用不包含</h3>
                        <p>1.旅行業為服務業，所以小費一直是領隊和司機的主要收入之一，世界各國皆如此，台灣也不例外。所以小費一直是領隊和司機的主要收入之一主要收入之一以下為建議小費，敬請參考：每人每天100元，二天共200元。</p>
                        <p>2.個人因素所產生之消費，如飲料、酒類、私人購物費…等。</p>
                        <p>3.個人旅遊平安保險，依規定旅客若有個別需求，得自行投保旅行平安保險。</p>
                        <p>4.本行程表上未註明之各項開銷，建議、自費或自由行程所衍生之任何費用。建議、自費或自由行程所衍生之任何費用。建議、自費或自由行程所衍生費用。</p>
                    </div>`;

        $.each(tripInfo.dateTravelPicList, function (dateIndex, dateTravelPicInfo) {
            dateReducedTravelItem += `
                    <tr>
                        <td class="water_blue">第${dateIndex + 1}天</td>
            `;
            dateTravelItem += `
                    <div class="timeline_travel">
                    <h3>第${dateIndex + 1}天</h3>
                    <div class="timeline_box clearfix">`;
            var dateTravelPicItem = "";
            var viewPointStr = "";
            var mealStr = `早餐：${dateTravelPicInfo.mealInfo.breakfast}<br />午餐：${dateTravelPicInfo.mealInfo.lunch}<br />晚餐：${dateTravelPicInfo.mealInfo.dinner}`;
            var hotelStr = `${dateTravelPicInfo.hotel}`;
            $.each(dateTravelPicInfo.travelPicList, function (picIndex, travelPicInfo) {
                if (picIndex % 2 == 0) {
                    dateTravelPicItem += `<div class="timeline_detail">`;
                } else {
                    dateTravelPicItem += `<div class="timeline_detail">`;
                }
                dateTravelPicItem += `
                    <div class="timeline_content">
                        <img src="/images/travel_photo1.jpg" alt="旅遊景點照片" title="鼻頭角遊憩區">
                        <h4>${travelPicInfo.travelPicTraditionalTitle}</h4>
                        <p>${travelPicInfo.travelPicTraditionalIntro}</p>
                    </div></div>`;

                if (picIndex == (dateTravelPicInfo.travelPicList.length - 1)) {
                    viewPointStr += `${travelPicInfo.travelPicTraditionalTitle}`;
                } else {
                    viewPointStr += `${travelPicInfo.travelPicTraditionalTitle}<br />`;
                }
            });

            dateReducedTravelItem += `
                        <td class="td_bottom">${viewPointStr}</td>
                        <td class="td_bottom">${mealStr}</td>
                        <td class="td_bottom">${hotelStr}</td>
                    </tr>
            `;

            dateTravelItem += dateTravelPicItem;
            dateTravelItem += `</div></div>`;
        });

        // localstorage 存儲旅遊統計 
        localStorage.setItem("travelStatisticsList", JSON.stringify(tripInfo.travelStatisticList));

        // 整理月份旅遊統計    
        $.each(tripInfo.travelStatisticList, function (monIndex, travelMonStatisticInfo) {
            var mon = travelMonStatisticInfo.travelMon;
            var monStringArr = mon.split("-");
            var dayList = ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六'];
            if (monIndex == 0) {
                travelMonStatisticsItem += `<div id="month_choose_tab_${monIndex + 1}" class="month_travel month_content month_content_${monIndex + 1} month_current" style="display: block">`;
            } else {
                travelMonStatisticsItem += `<div id="month_choose_tab_${monIndex + 1}" class="month_travel month_content month_content_${monIndex + 1}" style="display: none">`;
            }
            $.each(travelMonStatisticInfo.travelStatisticList, function (dateIndex, travelStatisticInfo) {
                var travelStepCode = travelStatisticInfo.travelStep;
                console.log(travelStatisticInfo);
                console.log(travelStepCode);
                if (dateIndex == 0) {
                    travelMonStatisticsItem += `<div class="month_travel_list selected clearfix">`;
                } else {
                    travelMonStatisticsItem += `<div class="month_travel_list clearfix">`;
                }
                if (travelStatisticInfo.travelStepSelectFlag == 1) {
                    travelStatisticsItem += `
                        <tr><th>去程時間</th><td>${travelStatisticInfo.startDate}</td></tr>
                        <tr><th>回程時間</th><td>${travelStatisticInfo.endDate}</td></tr>
                        <tr><th>天數</th><td>${travelStatisticInfo.dayNum}天</td></tr>
                        <tr><th>團數</th><td>${travelStatisticInfo.travelStep}</td></tr>
                        <tr><th>可賣</th><td>${travelStatisticInfo.remainSeatNum}</td></tr>
                        <tr><th>團位</th><td>${travelStatisticInfo.travelNum}</td></tr>
                        <tr><th>旅費</th><td><span class="dollars">${travelStatisticInfo.travelCost}</span>元起</td></tr>
                    `;
                }
                var dateStringArr = travelStatisticInfo.startDate.split("-");
                var day = new Date(Date.parse(travelStatisticInfo.startDate.replace(/-/g, '/'))).getDay();
                travelMonStatisticsItem += `
                    <input type="hidden" id="travel_step_code" value="${travelStepCode}">
                    <div class="week_day month_f_left">
                        <p class="day">${dateStringArr[2]}</p>
                        <p>${dayList[day]}</p>
                    </div>
                    <div class="week_day_seat month_f_left">
                        <p>可賣:${travelStatisticInfo.remainSeatNum}</p>
                        <p>團位:${travelStatisticInfo.travelNum}</p>
                    </div>
                    <div class="week_day_assign month_f_left">
                        <p><a href="#">報名</a></p>
                        <p><span class="dollars">$${travelStatisticInfo.travelCost}</span>元</p>
                    </div>
                </div>`;
            });
            travelMonStatisticsItem += `</div>`;

            if (monIndex == 0) {
                travelStatisticsMonItem += `<li class="two_day_current" >`;
                tabNum = monIndex + 1;
            } else {
                travelStatisticsMonItem += `<li class="">`;
            }
            travelStatisticsMonItem += `<div class="month_tab_${monIndex + 1}"><span>${monStringArr[0]}</span>年<span>${monStringArr[1]}</span>月</div></li>`;
        });

        $(".travel_title_text").html(travelTitleItem);
        $(".month_choose").append(travelStatisticsMonItem);
        $(".travel_info_post_title").html(travelTitle);
        $(".banner_title").html(bannerTitle);
        $(".bread_title").html(breadTitle);
        $(".calendar_list").append(travelMonStatisticsItem);
        $(".travel_info_post_table").html(travelStatisticsItem);
        $(".travel_content_text").html(travelContentItem);
        $(".timeline_travel_content").html(dateTravelItem);
        $(".order_table").html(dateReducedTravelItem);
        $("#tab_2").html(dateTravelCostItem);
    });
}

// 更新多日旅遊統計模塊
function updateTripStatisticInfo(travelStepCode = "") {
    // localstorage 取得旅遊統計 
    var travelStatisticsList = JSON.parse(localStorage.getItem('travelStatisticsList'));
    console.log(travelStatisticsList);
    var updateTravelStatisticsItem = "";
    $.each(travelStatisticsList, function (monIndex, travelMonStatisticInfo) {
        $.each(travelMonStatisticInfo.travelStatisticList, function (dateIndex, travelStatisticInfo) {
            if (travelStatisticInfo.travelStep == travelStepCode) {
                updateTravelStatisticsItem += `
                    <tr><th>去程時間</th><td>${travelStatisticInfo.startDate}</td></tr>
                    <tr><th>回程時間</th><td>${travelStatisticInfo.endDate}</td></tr>
                    <tr><th>天數</th><td>${travelStatisticInfo.dayNum}天</td></tr>
                    <tr><th>團數</th><td>${travelStatisticInfo.travelStep}</td></tr>
                    <tr><th>可賣</th><td>${travelStatisticInfo.remainSeatNum}</td></tr>
                    <tr><th>團位</th><td>${travelStatisticInfo.travelNum}</td></tr>
                    <tr><th>旅費</th><td><span class="dollars">${travelStatisticInfo.travelCost}</span>元起</td></tr>
                `;
            }
        });
    });
    $(".travel_info_post_table").html(updateTravelStatisticsItem);
}

// 取得video API 模塊  
function videoList() {
    var videoItem = `
            <div class="slick-slide">
                <img class="multiple_day_video_photo" src="/images/multipleday_video.png" alt="行程影片" title="行程影片">
            </div>
            <div class="slick-slide">
                <img class="multiple_day_video_photo" src="/images/multipleday_video.png" alt="行程影片" title="行程影片">
            </div>
            <div class="slick-slide">
                <img class="multiple_day_video_photo" src="/images/multipleday_video.png" alt="行程影片" title="行程影片">
            </div>`;
    $(".video_slider").append(videoItem);
    $('.slick-slider').slick('refresh');
}
