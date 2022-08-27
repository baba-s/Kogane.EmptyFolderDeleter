using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kogane.Internal
{
    /// <summary>
    /// すべての空フォルダを削除するエディタ拡張
    /// </summary>
    internal static class EmptyFolderDeleter
    {
        //================================================================================
        // 定数
        //================================================================================
        private const string ITEM_NAME_ROOT = "Kogane/";
        private const string ASSETS_PATH    = "Assets";

        //================================================================================
        // 関数（static）
        //================================================================================
        /// <summary>
        /// Unity メニューからすべての空フォルダを削除するための関数
        /// </summary>
        [MenuItem( ITEM_NAME_ROOT + "すべての空フォルダ削除" )]
        private static void DeleteFromMenu()
        {
            var isOk = EditorUtility.DisplayDialog
            (
                title: "",
                message: "すべての空フォルダを削除しますか？",
                ok: "OK",
                cancel: "Cancel"
            );

            if ( !isOk ) return;

            var result = Delete().ToArray();

            if ( !result.Any() )
            {
                EditorUtility.DisplayDialog
                (
                    title: "",
                    message: "空フォルダは存在しませんでした",
                    ok: "OK"
                );

                return;
            }

            EditorUtility.DisplayDialog
            (
                title: "",
                message: "すべての空フォルダを削除しました\n削除したフォルダのパスはコンソールに出力されます",
                ok: "OK"
            );

            Debug.Log( $"以下の空フォルダを削除しました\n{string.Join( "\n", result )}" );
        }

        /// <summary>
        /// Assets フォルダ以下のすべての空フォルダを削除します
        /// </summary>
        private static IReadOnlyList<string> Delete()
        {
            return Delete( ASSETS_PATH );
        }

        /// <summary>
        /// 指定されたフォルダ以下のすべての空フォルダを削除します
        /// </summary>
        private static IReadOnlyList<string> Delete( string path )
        {
            var list = new List<string>();

            foreach ( var n in GetList( path ) )
            {
                var isSuccess = AssetDatabase.DeleteAsset( n );

                if ( !isSuccess ) continue;

                list.Add( n );
            }

            return list;
        }

        /// <summary>
        /// 指定されたフォルダ以下のすべての空フォルダのパスを返します
        /// </summary>
        private static IEnumerable<string> GetList( string path )
        {
            foreach ( var dir in Directory.GetDirectories( path ).Select( x => x.Replace( "\\", "/" ) ) )
            {
                foreach ( var n in GetList( dir ) )
                {
                    yield return n;
                }

                var files = Directory
                        .GetFiles( dir )
                        .Where( x => !x.EndsWith( "/.DS_Store" ) )
                        .ToArray()
                    ;

                if ( files.Length != 0 ) continue;

                var dirs = Directory.GetDirectories( dir );

                if ( dirs.Length != 0 ) continue;

                yield return dir;
            }
        }
    }
}