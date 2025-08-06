# hikuka_hikanaika_server.py
# HikukaHikanaikaゲームのWeb APIサーバー
# Flaskを使用してRESTful APIを提供し、Unityからアクセス可能にする

from flask import Flask, request, jsonify
from flask_cors import CORS  # CORSをインポート（クロスオリジンリクエストを許可）
from hikuka_hikanaika_core import HikukaHikanaikaCore

# Flaskアプリケーションを初期化
app = Flask(__name__)

# CORSを設定（すべてのルートでクロスオリジンリクエストを許可）
CORS(app)

# HikukaHikanaikaコアクラスのインスタンスを作成
game_core = HikukaHikanaikaCore()

@app.route('/character/introduce', methods=['POST'])
def introduce_character():
    """キャラクター自己紹介エンドポイント
    
    Required JSON payload:
        character_name: キャラクター名
        beauty_stats: 美容ステータス情報
        
    Returns:
        JSON: キャラクターの自己紹介メッセージ
    """
    try:
        data = request.json
        if not data or 'character_name' not in data:
            return jsonify({"error": "character_nameが必要です"}), 400
            
        character_name = data['character_name']
        beauty_stats = data.get('beauty_stats', {})
        
        result = game_core.introduce_character(character_name, beauty_stats)
        return jsonify(result)
    except Exception as e:
        return jsonify({"error": str(e)}), 500

@app.route('/reality_show/explain', methods=['POST'])
def explain_reality_show():
    """リアリティーショー解説エンドポイント
    
    Required JSON payload:
        contestants: 参加者リスト（各参加者の情報を含む）
        
    Returns:
        JSON: リアリティーショーの解説メッセージ
    """
    try:
        data = request.json
        if not data or 'contestants' not in data:
            return jsonify({"error": "contestantsが必要です"}), 400
            
        contestants = data['contestants']
        if not isinstance(contestants, list):
            return jsonify({"error": "contestantsはリスト形式である必要があります"}), 400
            
        result = game_core.explain_reality_show(contestants)
        return jsonify(result)
    except Exception as e:
        return jsonify({"error": str(e)}), 500

@app.route('/characters', methods=['GET'])
def get_characters():
    """利用可能なキャラクター一覧取得エンドポイント
    
    Returns:
        JSON: キャラクター名のリスト
    """
    try:
        characters = game_core.get_character_list()
        return jsonify({"characters": characters})
    except Exception as e:
        return jsonify({"error": str(e)}), 500

@app.route('/character/<character_name>', methods=['GET'])
def get_character_info(character_name):
    """指定キャラクターの詳細情報取得エンドポイント
    
    Args:
        character_name (str): キャラクター名
        
    Returns:
        JSON: キャラクターの詳細情報
    """
    try:
        character_info = game_core.get_character_info(character_name)
        if not character_info:
            return jsonify({"error": "指定されたキャラクターが見つかりません"}), 404
            
        return jsonify({
            "character_name": character_name,
            "character_info": character_info
        })
    except Exception as e:
        return jsonify({"error": str(e)}), 500

@app.route('/reality_show/simulate', methods=['POST'])
def simulate_reality_show():
    """リアリティーショー全体のシミュレーションエンドポイント
    
    Required JSON payload:
        characters: キャラクター名のリスト
        beauty_stats_list: 各キャラクターの美容ステータス
        
    Returns:
        JSON: 全キャラクターの自己紹介とショー解説
    """
    try:
        data = request.json
        if not data or 'characters' not in data or 'beauty_stats_list' not in data:
            return jsonify({"error": "charactersとbeauty_stats_listが必要です"}), 400
            
        characters = data['characters']
        beauty_stats_list = data['beauty_stats_list']
        
        if len(characters) != len(beauty_stats_list):
            return jsonify({"error": "キャラクター数と美容ステータス数が一致しません"}), 400
        
        # 各キャラクターの自己紹介を生成
        contestant_introductions = []
        for i, character_name in enumerate(characters):
            beauty_stats = beauty_stats_list[i]
            intro_result = game_core.introduce_character(character_name, beauty_stats)
            contestant_introductions.append(intro_result)
        
        # リアリティーショーの解説を生成
        show_explanation = game_core.explain_reality_show(contestant_introductions)
        
        return jsonify({
            "contestant_introductions": contestant_introductions,
            "show_explanation": show_explanation
        })
    except Exception as e:
        return jsonify({"error": str(e)}), 500

# ヘルスチェックエンドポイント（サーバーが動作しているか確認）
@app.route('/health', methods=['GET'])
def health_check():
    """サーバーのヘルスチェック
    
    Returns:
        JSON: サーバーの状態
    """
    return jsonify({"status": "OK", "message": "HikukaHikanaikaサーバーが稼働中です！"})

@app.route('/', methods=['GET'])
def root():
    """ルートエンドポイント - API情報を返す
    
    Returns:
        JSON: API情報
    """
    return jsonify({
        "game": "HikukaHikanaika",
        "description": "リアリティーショーキャラクター対話API",
        "endpoints": {
            "GET /": "API情報",
            "GET /health": "ヘルスチェック",
            "GET /characters": "キャラクター一覧取得",
            "GET /character/<name>": "キャラクター詳細情報取得",
            "POST /character/introduce": "キャラクター自己紹介",
            "POST /reality_show/explain": "リアリティーショー解説",
            "POST /reality_show/simulate": "リアリティーショー全体シミュレーション"
        }
    })

if __name__ == '__main__':
    print("HikukaHikanaika APIサーバーを起動中...")
    print("URL: http://localhost:5001")
    print("エンドポイント:")
    print("  GET  / - API情報")
    print("  GET  /health - ヘルスチェック")
    print("  GET  /characters - キャラクター一覧")
    print("  GET  /character/<name> - キャラクター詳細")
    print("  POST /character/introduce - キャラクター自己紹介")
    print("  POST /reality_show/explain - リアリティーショー解説")
    print("  POST /reality_show/simulate - ショー全体シミュレーション")
    
    print("\n📝 使用例:")
    print("  curl http://localhost:5001/characters")
    print("  curl http://localhost:5001/character/美咲")
    print('  curl -X POST http://localhost:5001/character/introduce \\')
    print('       -H "Content-Type: application/json" \\')
    print('       -d \'{"character_name": "美咲", "beauty_stats": {"fashion": "オートクチュールのドレス"}}\'')
    print()
    
    # サーバーを起動（デバッグモード、すべてのIPからアクセス可能、ポート5001）
    app.run(debug=True, host='0.0.0.0', port=5001)