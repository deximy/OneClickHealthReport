<script lang="ts" setup>
import * as signalR from "@microsoft/signalr";

import {ref} from "vue";
import {NSpin} from "naive-ui";
import {useMessage as UseMessage} from "naive-ui/es";

const message = UseMessage();

const connection = new signalR.HubConnectionBuilder().withUrl("http://localhost:5111/login").build();
connection.on(
    "ReceiveQrCode",
    (qr_code: string) => {
        show.value = false;
        src.value = "data:image/png;base64," + qr_code;
        console.log("Qrcode received!");
    }
);
connection.on(
    "ReceiveAuthCode",
    (auth_code: string) => {
        console.log("auth_code: " + auth_code);
    }
);
connection.on(
    "ReceiveKey",
    (key: string) => {
        console.log("key: " + key);
    }
);
connection.on(
    "ReceiveResult",
    (result: boolean) => {
        if (result)
        {
            message.success("提交成功！");
        }
    }
);
connection
    .start()
    .catch(
        (err) => {
            show.value = false;
            message.error("连接服务器失败，请刷新重试……");
            console.error(err);
        }
    );

const src = ref("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAZoAAAGaCAIAAAC5ZBI0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAUZSURBVHhe7dQBDQAADMOg+ze9+2hABDeABJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BEToDInQGROgMiNAZEKEzIEJnQITOgAidARE6AyJ0BkToDIjQGRChMyBCZ0CEzoAInQEROgMidAZE6AyI0BkQoTMgQmdAhM6ACJ0BCdsDCYatC7QYMykAAAAASUVORK5CYII=");
const show = ref(true);
</script>

<template>
    <div class="qr-code-viewer">
        <span class="tips" v-show="!show">使用企业微信扫描下方二维码即可自动重复上一次打卡数据</span>
        <n-spin class="img-wrapper" :show="show">
            <img :src="src" alt="">
            <template #description>加载中……</template>
        </n-spin>
    </div>
</template>

<style scoped>
.qr-code-viewer
{
    display: flex;
    flex-direction: column;
    align-items: center;
}
.tips
{
    margin-bottom: 20px;
    font-size: medium;
}
.img-wrapper
{
    height: 410px;
    width: 410px;
}
</style>