
$(function () {
    $.post('/Member/getMemberInfo', { memberCode: "mem98808591"}).done(function (memberInfo) {
        var memberData = `
            <tr>
            <th class="member_title">身分證帳號</th>
            <td class="member_text">${memberInfo.username}</td>
            <th class="member_title">聯絡地址</th>
            <td class="member_text">${memberInfo.address}</td>
            </tr>
            <tr>
                <th class="member_title">姓名</th>
                <td class="member_text">${memberInfo.name}</td>
                <th class="member_title">電子信箱</th>
                <td class="member_text">${memberInfo.email}</td>
            </tr>
            <tr>
                <th class="member_title">出生日期</th>
                <td class="member_text">${memberInfo.birthday}</td>
                <th class="member_title">緊急聯絡人姓名</th>
                <td class="member_text">${""}</td>
            </tr>
            <tr>
                <th class="member_title">聯絡電話</th>
                <td class="member_text">${memberInfo.phone}</td>
                <th class="member_title">緊急聯絡人電話</th>
                <td class="member_text">${""}</td>
            </tr>
            <tr>
                <th class="member_title">聯絡手機</th>
                <td class="member_text">${memberInfo.phone}</td>
                <th class="member_title"></th>
                <td class="member_text"></td>
            </tr>`;
        $('.member_table').append(memberData);
    });

    $.post('/Member/getMemberReservationList', { memberCode: "memberCode00001", page: 1, limit: 100 }).done(function (memberReservationList) {
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
});
