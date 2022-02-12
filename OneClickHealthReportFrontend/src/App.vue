<script setup lang="ts">
import QrCodeViewer from "./components/QrCodeViewer.vue";
import {NMessageProvider} from "naive-ui";
</script>

<template>
    <div class="viewer-wrapper">
        <n-message-provider>
            <qr-code-viewer class="qr-code-viewer" />
        </n-message-provider>
    </div>

</template>

<style scoped>
.viewer-wrapper
{
    display: flex;
    flex: 1;
    justify-content: center;
    flex-direction: column;
}
</style>
<style>
html, body, #app
{
    height: 100%;
    width: 100%;
    margin: 0;
}
#app
{
    display: flex;
}
</style>
