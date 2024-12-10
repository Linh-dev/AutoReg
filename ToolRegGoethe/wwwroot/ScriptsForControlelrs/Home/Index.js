app.controller('homeIndexCtrl', function ($scope, $mdDialog, $http, $timeout, $templateCache) {
    $scope.message = "Tool đăng ký thi tiếng";

    $scope.hide = function () {
        $mdDialog.hide();
    };
    $scope.cancel = function () {
        $mdDialog.cancel();
    };

    $scope.init = function () {
        initDropzone();
        $scope.searchFullText();
    }

    $scope.uploadFile = function () {
        $('#myDropzone').trigger('click');
    }

    $scope.searchFullText = function () {
        //tao object tim kiem        
        var reqData =
        {

        };
        $http.post("/Home/GetAll", reqData)
            .then(function (rs) {
                if (rs.data.returnCode == "1") {
                    $scope.Data = rs.data.data;
                }
                else {
                    $scope.Data = [];
                }
            })
    };

    $scope.startReg = function (obj) {
        var reqData =
        {
            idStr: obj.idStr
        };
        $http.post("/Home/StartReg", reqData)
            .then(function (rs) {
                if (rs.data.returnCode == "1") {
                    $scope.hide();
                }
                else {
                    $scope.cancel();
                }
            });
    }

    $scope.cancel = function () {
        $http.post("/Home/Cancel")
            .then(function (rs) {
            });
    }

    $scope.showDeleteModal = function (ev, obj) {
        $mdDialog.show({
            controller: ConfirmController,
            template:
                '<md-dialog aria-label="List dialog">' +
                '  <md-dialog-content>' +
                '     <div class="modal-header modal-header1 modal-header-del">' +
                '		    <div class="delete-icon"></div>' +
                '	</div>' +
                '	<div class="modal-body modal-body-del">' +
                '	    Bạn có muốn xóa bản ghi này không?' +
                '	</div>' +
                '	<div class="modal-footer modal-footer-del">' +
                '	<button type="button" class="btn btn-red" ng-click="removeItem()"><i class="fa fa-trash-alt"></i>Xóa</button>' +
                '	<button type="button" class="btn btn-gray" data-dismiss="modal" ng-click="cancel()"><i class="fa fa-times pr-2"></i>Hủy</button>' +
                '		</div>' +
                '  </md-dialog-content>' +
                '</md-dialog>',
            parent: angular.element(document.body),
            locals: {
                obj: obj,
                rootScope: $scope
            },
            targetEvent: ev,
            flex: "100",
            clickOutsideToClose: true
        })
            .then(function () {
                $scope.searchFullText();
            }, function () { });
    };
    function ConfirmController($scope, $mdDialog, obj, rootScope) {
        $scope.obj = obj;
        $scope.rootScope = rootScope;
        $scope.hide = function () {
            $mdDialog.hide();
        };
        $scope.cancel = function () {
            $mdDialog.cancel();
        };
        $scope.removeItem = function () {
            var reqData =
            {
                idStr: $scope.obj.idStr
            };
            $http.post("/Home/DeteleJson", reqData)
                .then(function (rs) {
                    if (rs.data.returnCode == "1") {
                        $scope.hide();
                    }
                    else {
                        $scope.cancel();
                    }
                });
        };
    }

    function initDropzone() {
        $('#myDropzone').addClass('dropzone');

        $("div#myDropzone").dropzone({
            url: "/home/UploadExcelFile",
            acceptedFiles: ".xlsx,.xls",
            maxFiles: 1,
            maxFilesize: 50,
            addRemoveLinks: true,
            maxfilesexceeded: function (file) {
            },
            createImageThumbnails: false, // NO THUMBS!
            init: function () {
                var prevFile;
                this.on("error", function (file, message) {
                    this.removeFile(file);
                    console.log(message);
                });

                this.on('addedfile', function (file) {

                    if (typeof prevFile != "undefined" && prevFile != null) {
                        this.removeFile(prevFile);
                    }
                });

                this.on('removedfile', function (file) {
                    prevFile = null;
                });

                this.on('resetFiles', function () {
                    if (this.files.length != 0) {
                        for (i = 0; i < this.files.length; i++) {
                            this.files[i].previewElement.remove();
                        }
                        this.files.length = 0;
                    }
                    prevFile = null;
                });

                this.on('success', function (file, responseText) {
                    if (typeof prevFile !== "undefined" && prevFile != null) {
                        this.removeFile(prevFile);
                    }

                    if ($("#EditDivImageLocation .dz-preview").length > 1) {
                        $("#EditDivImageLocation .dz-preview:first").remove();
                    }
                    prevFile = file;
                });
            },
            success: function (e, data) {
                console.log(data);
                $scope.searchFullText();
            }
        });
    }


    $scope.init();
});