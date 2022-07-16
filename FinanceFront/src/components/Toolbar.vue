<template>
    <div id="main-toolbar" class="center">
        <CustomButton :text="Fondos" :action="fondos_onClick"></CustomButton>
    </div>
</template>

<script lang="js">
import CustomButton from './utils/CustomButton.vue'

export default {
    name: 'Toolbar',
    components: {
        CustomButton
    },
    methods: {
        fondos_onClick() {
            alert("Fondos");
        }
    }
};
</script>

<style>
#main-toolbar {
    display: flex;
    flex-flow: row;
    width: 100vw;
    background-color: red;
    padding: 8px;
    align-items: center;
    justify-content: center;
    vertical-align: middle;
}
</style>
