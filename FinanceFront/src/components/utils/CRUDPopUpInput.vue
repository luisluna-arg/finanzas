<template>
    <div>
        <label for:={{ settings.id }}>{{ settings.label }}</label>
        <input id:={{ settings.id }} type="text" />
    </div>
</template>

<script>
export default {
    props: {
        settings: Object
    },
    data: function () {
        console.log(this.props);
        return {
            items: []
        };
    }
}
</script>

<style>
</style>