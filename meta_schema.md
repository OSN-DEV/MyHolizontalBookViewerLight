{
	"$schema": "http://json-schema.org/draft-07/schema#",
	"title": "書籍定義",
	"type": "object",
	"properties": {
		"id" : {
			"type": "integer",
			"description": "書籍の識別子"
		},
		"type": {
			"type": "string",
			"enum": [
				"reflow",
				"fix"
			],
			"description": "reflow:リフローレイアウト、fix:固定レイアウト"
		},
		"orientation": {
			"type": "string",
			"enum": [
				"horizontal",
				"vertical"
			],
			"description": "horizontal:縦書き、vertical:横書き。リフローレイアウトのときに指定"
		},
		"direction": {
			"type": "string",
			"enum": [
				"left",
				"right"
			],
			"description": "left:右→左、right:左→右"
		},
		"title": {
			"type": "string",
			"description": "書籍のタイトル"
		},
		"titleRuby": {
			"type": "string",
			"description": "書籍のタイトル(ふりがな)"
		},
		"cover": {
			"type": "string",
			"description": "書籍のカバー"
		},
		"authers": {
			"type": "array",
			"items": {
				"type": "object",
				"properties": {
					"author": {
						"type": "string",
						"description": "著者名"
					},
					"authorRuty": {
						"type": "string",
						"description": "著者名(ふりがな)"
					},
				}
			}
		},
		"tags": {
			"type": "array",
			"items" : {
				"type": "string"
			},
			"description": "タグ"
		},
		"characters": {
			"type": "string",
			"description": "登場人物紹介ページへ"
		},
		"tableOfContents": {
			"type": "array",
			"items": {
				"type": "object",
				"properties": {
					"level": {
						"type": "integer",
						"enum": [
							1,2,3
						],
						"description": "見出しの階層"
					},
					"content": {
						"type": "string",
						"description": "目次のタイトル"
					},
					"link": {
						"type": "string",
						"description": "ページのリンク"
					}
				},
				"required": ["level", "content"]
			}
		}
	},
	"required": ["id", "type", "direction", "title"]
}