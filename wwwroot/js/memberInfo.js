  
$(function () {

    // 判斷是否已登入，沒登入則跳回首頁  
    var loginInfo = JSON.parse(localStorage.getItem('loginInfo'));
    var loginFlag = loginFlagMethod(loginInfo);
    if (!loginFlag) {
        window.location.href = '/';
    }

    // 初始化取得會員資訊 
    getMemberInfo(loginInfo.memberCode);

    // 取得會員訂位列表
    getMemberReservationList(loginInfo.memberCode);

    // 按鈕 update memberInfo API 
    $(".adjust_btn").on("click", function () {
        updateMemberInfo();
    });
});

// 取得會員資訊模塊
function getMemberInfo(memberCode = "") {
    $.post('/Member/getMemberInfo', { memberCode: memberCode }).done(function (memberInfo) {
        var memberData = `
            <tr>
            <th class="member_title">身分證帳號</th>
            <td class="member_text">${memberInfo.username}</td>
            <th class="member_title">聯絡地址</th>
            <td class="member_text"><input type="text" id="member_addr" name="member_addr" value=${memberInfo.address}></td>
            </tr>
            <tr>
                <th class="member_title">姓名</th>
                <td class="member_text"><input type="text" id="member_name" name="member_name" value=${memberInfo.name}></td>
                <th class="member_title">電子信箱</th>
                <td class="member_text"><input type="text" id="member_email" name="member_email" value=${memberInfo.email}></td>
            </tr>
            <tr>
                <th class="member_title">出生日期</th>
                <td class="member_text"><input type="date" id="member_birthday" name="member_birthday" value=${memberInfo.birthday}></td>
                <th class="member_title">緊急聯絡人姓名</th>
                <td class="member_text"><input type="text" id="member_emer_name" name="member_emer_name" value=${memberInfo.emername}></td>
            </tr>
            <tr>
                <th class="member_title">聯絡電話</th>
                <td class="member_text"><input type="text" id="member_phone" name="member_phone" value=${memberInfo.phone}></td>
                <th class="member_title">緊急聯絡人電話</th>
                <td class="member_text"><input type="text" id="member_emer_phone" name="member_emer_phone" value=${memberInfo.emerphone}></td>
            </tr>
            <tr>
                <th class="member_title">聯絡手機</th>
                <td class="member_text">${memberInfo.cellphone}</td>
                <th class="member_title"></th>
                <td class="member_text"></td>
            </tr>`;
        $('.member_table').append(memberData);
    });
}

// 取得會員訂位列表模塊  
function getMemberReservationList(memberCode = "") {
    $.post('/Member/getMemberReservationList', { memberCode: memberCode, page: 1, limit: 100 }).done(function (memberReservationList) {
        var list = `
                <tr>
                    <th class="order_date">訂位日期</th>
                    <th class="group_no">出團梯次</th>
                    <th class="car_no">車次</th>
                    <th class="itineary">行程地點</th>
                    <th class="fee">費用說明</th>
                    <th class="cancel">取消行程</th>
                    <th class="cancel">匯款資訊</th>
                </tr>`;

        $.each(memberReservationList, function (i, memberReservationInfo) {
            list += `
            <tr>
                <td class="light_blue">${memberReservationInfo.travelReservationDate}</td>
                <td class="td_bottom">${memberReservationInfo.travelStepDate}</td>
                <td class="td_bottom">${""}</td>
                <td class="td_bottom">${memberReservationInfo.travelTraditionalTitle}</td>
                <td class="td_bottom">${""}</td>
                <td class="td_bottom td_button"><a href="#" class="cancel_btn">取消行程</a></td>
                <td class="td_bottom td_button"><a href="#" class="cancel_btn">上傳匯款證明</a></td>
            </tr>`;
        });
        $('.order_table').append(list);
    });
}

// 更新會員資訊模塊  
function updateMemberInfo() {
    var loginInfo = JSON.parse(localStorage.getItem('loginInfo'));
    var name = $("#member_name").val();
    var email = $("#member_email").val();
    var address = $("#member_addr").val();
    var birthday = $("#member_birthday").val();
    var emerContactName = $("#member_emer_name").val();
    var emerContactPhone = $("#member_emer_phone").val();
    var phone = $("#member_phone").val();
    $.post('/Member/updateMemberInfo', { memberCode: loginInfo.memberCode, name: name, email: email, address: address, phone: phone, birthday: birthday, emerContactName: emerContactName, emerContactPhone: emerContactPhone }).done(function (updateRes) {
        window.location.href = '/Member/MemberInfo';
    });
}
