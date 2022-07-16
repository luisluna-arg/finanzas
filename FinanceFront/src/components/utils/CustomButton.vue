<template>
    <button @click="action">{{ text }}</button>
</template>

<script>
export default {
    name: 'CustomButton',
    props: ['text', 'action'],
    data() {
        return {
            name: 'John Doe'
        }
    },
}
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
