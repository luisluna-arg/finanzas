<template>
    <div>
        <b-form-group v-bind:id="settings.id + '-group'" v-bind:label="settings.label" label-for="settings.id"
            v-bind:description="settings.description">
            <b-form-input v-bind:id="settings.id" v-bind:type="settings.type" v-bind:placeholder="settings.placeholder"
                v-model="model" @input="onInput" required />
        </b-form-group>
    </div>
</template>

<script>
export default {
    methods: {
        onInput(value) {
            this.$emit('update:modelValue', value);
        }
    },
    props: {
        settings: Object
    },
    data: function () {
        // console.log(this);
        return {
            items: []
        };
    }
}
</script>

<style>
</style>