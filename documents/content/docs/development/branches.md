---
title: "ブランチ構成"
weight: 10
type: docs
---

## ブランチ構成

### develop
デフォルトブランチです。Pull Requestは原則、このブランチに向けて送ってください。

### main
リリースされているCOCOAのソースコードの最新版が格納されます。各バージョンのコードは[Release](https://github.com/cocoa-mhlw/cocoa/releases)にあります。

### featureブランチ
大きめの機能を作る際、複数人数で作業を分担するために`cocoa-mhlw/cocoa`上に作ります。featureブランチがある場合、関連するPull Requestは該当するfeatureブランチに向けます。作業の完了後、`develop`向けにPull Reqeustを出す流れです。
featureを作るか、developをIssueやPull Requestの大きさを見ながらコラボレーターが判断します。詳細は相談してください。
