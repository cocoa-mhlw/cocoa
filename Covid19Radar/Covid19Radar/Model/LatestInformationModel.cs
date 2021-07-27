/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#nullable enable

using System;
using Covid19Radar.Resources;

namespace Covid19Radar.Model
{
    /// <summary>
    ///  最新情報を表します。
    /// </summary>
    public sealed class LatestInformationModel
    {
        /// <summary>
        ///  最新情報の題名を取得または設定します。
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        ///  最新情報の投稿日時を取得または設定します。
        /// </summary>
        public DateTime Posted { get; set; }

        /// <summary>
        ///  最新情報の投稿日時を文字列として取得します。
        /// </summary>
        public string PostedAsString => this.Posted.ToString(AppResources.NewsPageLabel_Posted_Format);

        /// <summary>
        ///  最新情報の内容を取得または設定します。
        /// </summary>
        public string? Contents { get; set; }

        /// <summary>
        ///  最新情報のタグを含む配列を取得または設定します。
        /// </summary>
        public string?[]? Tags { get; set; }
    }
}
