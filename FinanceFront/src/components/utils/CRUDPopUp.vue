<template>
    <b-button id="show-btn" @click="openModal">Open Modal</b-button>

    <b-modal ref="my-modal" hide-footer title="Using Component Methods" v-model="modalShow">
        <b-form v-bind:id="formId" @submit="accept" @reset="cancel">
            <CRUDPopUpInput v-for="fieldSettings in editorSettings" :key="fieldSettings.id" v-bind="fieldSettings"
                :settings="fieldSettings" v-model="form[fieldSettings.id]" />

            <b-button type="submit" variant="primary">Aceptar</b-button>
            <b-button type="reset" variant="danger">Cancelar</b-button>
        </b-form>
    </b-modal>
    <!-- </div> -->
</template>

<script>
import CRUDPopUpInput from '@/components/utils/CRUDPopUpInput.vue'

let form = {};

let sendForm = function (formValues, callback) {
    console.log(formValues);
    if (typeof callback == "function") {
        callback();
    }
}

export default {
    methods: {
        resetForm() {
            document.getElementById(this.formId).reset();
            for (let field in form) {
                delete form[field];
            }
        },
        showModal(isVisible) {
            this.modalShow = !!isVisible;
        },
        openModal() {
            this.showModal(true);
        },
        cancel() {
            this.showModal(false);
            this.resetForm();
        },
        accept() {
            let showModalCallback = this.showModal;
            let resetFormCallback = this.resetForm;
            sendForm(this.form, function () {
                showModalCallback(false);
                resetFormCallback();
            });
        }
    },
    components: {
        CRUDPopUpInput,
    },
    props: {
        formId: String,
        editorSettings: Array
    },
    mounted: function () {

    },
    data: function () {
        return {
            //editorSettings: this.editorSettings,
            //show: false,
            form: form,
            modalShow: false
        };
    }
}
</script>

<style>
</style>