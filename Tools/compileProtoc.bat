@REM This Source Code Form is subject to the terms of the Mozilla Public
@REM License, v. 2.0. If a copy of the MPL was not distributed with this
@REM file, You can obtain one at https://mozilla.org/MPL/2.0/.


protoc --proto_path=..\src\Covid19Radar.Background\Protobuf\ --csharp_out=..\src\Covid19Radar.Background\Protobuf\ --csharp_opt=file_extension=.g.cs TemporaryExposureKeyExportFileFormat.proto

PAUSE
