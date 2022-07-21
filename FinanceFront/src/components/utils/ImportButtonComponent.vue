<template>
    <div>
        <input type="file" id="excelfile" accept=".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel" />  
        <input type="button" id="viewfile" value="Importar" @click="UploadContent" />  
            <br />  
            <br />  
        <table id="exceltable">  
        </table>     
    </div>
</template>

<script>

import { UploadUrls } from '@/utils/commons.js'

let sendFile = function (uploadType, file) {
    let uri = UploadUrls[uploadType];

    console.log(uploadType, uri, file, file.buffer);

    // axios.
    //     put(uri, file, { 
    //         headers: { 'Content-Type': file.type } 
    //         }).
    //     then(response => { 
    //         console.log('response:', response);
    //         });
}

export default {
    methods: {
        UploadContent: function() {
            var fileDom = document.getElementById("excelfile");
            for (let i = 0; i < fileDom.files.length; i++) {
                sendFile(this.UploadType, fileDom.files[i]);
            }
        }
    },
    props: {
        UploadType: Symbol
    }
}
</script>